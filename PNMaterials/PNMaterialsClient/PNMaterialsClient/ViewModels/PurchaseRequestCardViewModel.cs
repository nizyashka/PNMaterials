using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class PurchaseRequestCardViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    public PurchaseRequestCardViewModel(ApiClient api, ILogger<PurchaseRequestCardViewModel> logger)
        : base(logger)
    {
        _api = api;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    private int _id;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    private string _number = string.Empty;

    [ObservableProperty]
    private string _createdAt = "—";

    [ObservableProperty]
    private DateTimeOffset _deliveryDate = DateTime.Today.AddMonths(1);

    [ObservableProperty]
    private string _statusText = "—";

    [ObservableProperty]
    private bool _isEditMode;

    public ObservableCollection<MaterialDto> Materials { get; } = new();
    public ObservableCollection<RequestItemViewModel> Items { get; } = new();
    public ObservableCollection<StatusActionViewModel> StatusActions { get; } = new();

    public string Header => Id == 0
        ? "Новая заявка"
        : $"{Number} — заявка на закупку";

    public async Task LoadAsync(int id, int? materialId = null)
    {
        await RunAsync($"Загрузка заявки {id}", async () =>
        {
            await LoadMaterialsAsync();

            if (id == 0)
            {
                Id = 0;
                Number = string.Empty;
                CreatedAt = "—";
                StatusText = "—";
                DeliveryDate = DateTime.Today.AddMonths(1);

                Items.Clear();
                Items.Add(CreateItem());
                StatusActions.Clear();

                IsEditMode = true;

                if (materialId.HasValue)
                    PrefillMaterial(materialId.Value);

                return;
            }

            var request = await _api.GetPurchaseRequestAsync(id);

            if (request is null)
            {
                MessagesService.ShowWarning("Заявка не найдена.");
                return;
            }

            Apply(request);
            IsEditMode = false;
            MessagesService.ShowInfo("Данные перечитаны из базы.");
        });
    }

    private async Task LoadMaterialsAsync()
    {
        Materials.Clear();
        foreach (var m in await _api.GetMaterialsAsync())
            Materials.Add(m);
    }

    private void Apply(PurchaseRequestDto request)
    {
        Id = request.Id;
        Number = request.Number;
        CreatedAt = request.CreatedAt.ToString("dd.MM.yyyy HH:mm");
        DeliveryDate = request.DeliveryDate;
        StatusText = request.Status.Title;

        Items.Clear();
        foreach (var source in request.Items)
        {
            var item = CreateItem();
            item.Material = Materials.FirstOrDefault(m => m.Id == source.MaterialId);
            item.Quantity = (double)source.Quantity;
            item.PositionText = source.PositionText ?? string.Empty;
            Items.Add(item);
        }

        BuildStatusActions(request);
    }

    private RequestItemViewModel CreateItem()
    {
        var item = new RequestItemViewModel
        {
            IsEditMode = IsEditMode,
            Materials = Materials
        };

        item.RemoveCommand = new RelayCommand(() => Items.Remove(item));
        return item;
    }

    private void PrefillMaterial(int materialId)
    {
        var material = Materials.FirstOrDefault(m => m.Id == materialId);

        if (material is null)
        {
            MessagesService.ShowWarning("Материал не найден.");
            return;
        }

        if (Items.Count == 0)
            Items.Add(CreateItem());

        Items[0].Material = material;
    }

    private void BuildStatusActions(PurchaseRequestDto request)
    {
        StatusActions.Clear();

        foreach (var target in request.AllowedTransitions)
        {
            var targetId = target.Id;

            StatusActions.Add(new StatusActionViewModel(
                ActionText(request.Status.Code, target.Code),
                new AsyncRelayCommand(() => ChangeStatusAsync(targetId))));
        }
    }

    private static string ActionText(string currentCode, string targetCode) => targetCode switch
    {
        "ВРБТ" => currentCode == "ВПЛН" ? "Вернуть в работу" : "Направить в закупку",
        "ВПЛН" => "Закуплено",
        "УДЛН" => "Удалить",
        "СЗДН" => "Восстановить",
        _ => targetCode
    };

    private async Task ChangeStatusAsync(int statusId)
    {
        await RunAsync($"Смена статуса заявки {Id}", async () =>
        {
            var updated = await _api.ChangeRequestStatusAsync(Id, statusId);
            Apply(updated);
            MessagesService.ShowSuccess($"Статус изменён на {updated.Status.Code}.");
        });
    }

    [RelayCommand]
    private void AddItem() => Items.Add(CreateItem());

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new PurchaseRequestEditDto
        {
            DeliveryDate = DeliveryDate.Date,
            Items = Items.Select(i => new PurchaseRequestItemEditDto
            {
                MaterialId = i.Material?.Id ?? 0,
                Quantity = double.IsNaN(i.Quantity) ? 0 : (decimal)i.Quantity,
                PositionText = string.IsNullOrWhiteSpace(i.PositionText) ? null : i.PositionText
            }).ToList()
        };

        var quantityErrors = Items
            .Zip(dto.Items)
            .Where(pair => pair.First.Material is not null
                && pair.Second.Quantity is >= 0.01m and <= 999_999.99m
                && !QuantityRules.IsValid(pair.Second.Quantity, pair.First.UnitAllowsFractions))
            .Select(pair => pair.First.UnitAllowsFractions
                ? $"Количество материала «{pair.First.MaterialName}» ({pair.First.UnitName}) должно быть от 0,01 до 999 999,99, не больше двух знаков после запятой."
                : $"Количество материала «{pair.First.MaterialName}» ({pair.First.UnitName}) должно быть целым числом от 1 до 999 999.");

        var errors = DtoValidator.Validate(dto)
            .Concat(dto.Items.SelectMany(DtoValidator.Validate))
            .Concat(quantityErrors)
            .Distinct()
            .ToList();

        if (errors.Count > 0)
        {
            MessagesService.ShowWarning(string.Join(Environment.NewLine, errors));
            return;
        }

        await RunAsync("Сохранение заявки", async () =>
        {
            if (Id == 0)
            {
                var created = await _api.CreatePurchaseRequestAsync(dto);
                Apply(created);
                MessagesService.ShowSuccess($"Заявка создана, номер {created.Number}.");
            }
            else
            {
                await _api.UpdatePurchaseRequestAsync(Id, dto);

                var reloaded = await _api.GetPurchaseRequestAsync(Id);
                if (reloaded is not null)
                    Apply(reloaded);

                MessagesService.ShowSuccess("Изменения сохранены.");
            }

            IsEditMode = false;
        });
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (Id != 0 && !IsEditMode)
        {
            await LoadAsync(Id);
            return;
        }

        var selected = Items
            .Select(item => (Item: item, MaterialId: item.Material?.Id))
            .ToList();

        await RunAsync("Обновление списка материалов", async () =>
        {
            await LoadMaterialsAsync();

            foreach (var (item, materialId) in selected)
                item.Material = Materials.FirstOrDefault(m => m.Id == materialId);

            MessagesService.ShowSuccess("Список материалов обновлён.");
        });
    }

    partial void OnIsEditModeChanged(bool value)
    {
        foreach (var item in Items)
            item.IsEditMode = value;
    }
}
