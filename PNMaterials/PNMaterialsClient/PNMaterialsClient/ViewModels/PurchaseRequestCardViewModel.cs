using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PNMaterialsClient.ViewModels;

public partial class PurchaseRequestCardViewModel : ObservableObject
{
    private readonly ApiClient _api;

    public PurchaseRequestCardViewModel(ApiClient api)
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

    [ObservableProperty]
    private string? _status;

    public ObservableCollection<MaterialDto> Materials { get; } = new();
    public ObservableCollection<RequestItemViewModel> Items { get; } = new();
    public ObservableCollection<StatusActionViewModel> StatusActions { get; } = new();

    public string Header => Id == 0
        ? "Новая заявка"
        : $"{Number} — заявка на закупку";

    public async Task LoadAsync(int id)
    {
        try
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
                Status = "Номер будет присвоен системой после сохранения.";
                return;
            }

            var request = await _api.GetPurchaseRequestAsync(id);

            if (request is null)
            {
                Status = "Заявка не найдена.";
                return;
            }

            Apply(request);
            IsEditMode = false;
            Status = null;
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
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
        StatusText = $"{request.Status.Code} — {request.Status.Name}";

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

    public void PrefillMaterial(int materialId)
    {
        var material = Materials.FirstOrDefault(m => m.Id == materialId);

        if (material is null)
            return;

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
        try
        {
            var updated = await _api.ChangeRequestStatusAsync(Id, statusId);
            Apply(updated);
            Status = $"Статус изменён на {updated.Status.Code}.";
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private void AddItem() => Items.Add(CreateItem());

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Items.Count == 0)
        {
            Status = "Заявка должна содержать хотя бы одну позицию.";
            return;
        }

        if (Items.Any(i => i.Material is null))
        {
            Status = "В каждой позиции нужно выбрать материал.";
            return;
        }

        try
        {
            var dto = new PurchaseRequestEditDto
            {
                DeliveryDate = DeliveryDate.Date,
                Items = Items.Select(i => new PurchaseRequestItemEditDto
                {
                    MaterialId = i.Material!.Id,
                    Quantity = (decimal)i.Quantity,
                    PositionText = string.IsNullOrWhiteSpace(i.PositionText) ? null : i.PositionText
                }).ToList()
            };

            if (Id == 0)
            {
                var created = await _api.CreatePurchaseRequestAsync(dto);
                Apply(created);
                Status = $"Заявка создана, номер {created.Number}.";
            }
            else
            {
                await _api.UpdatePurchaseRequestAsync(Id, dto);

                var reloaded = await _api.GetPurchaseRequestAsync(Id);
                if (reloaded is not null)
                    Apply(reloaded);

                Status = "Изменения сохранены.";
            }

            IsEditMode = false;
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (Id == 0)
            return;

        await LoadAsync(Id);
        Status = "Данные перечитаны из базы.";
    }

    partial void OnIsEditModeChanged(bool value)
    {
        foreach (var item in Items)
            item.IsEditMode = value;
    }
}
