using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialCardViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    public MaterialCardViewModel(ApiClient api, ILogger<MaterialCardViewModel> logger)
        : base(logger)
    {
        _api = api;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    [NotifyPropertyChangedFor(nameof(CanCreateRequest))]
    [NotifyCanExecuteChangedFor(nameof(ToggleArchiveCommand))]
    private int _id;

    public bool CanCreateRequest => Id != 0 && !IsEditMode && !IsArchived;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    private string _code = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private UnitDto? _selectedUnit;

    [ObservableProperty]
    private MaterialGroupDto? _selectedGroup;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreateRequest))]
    [NotifyCanExecuteChangedFor(nameof(ToggleArchiveCommand))]
    private bool _isEditMode;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    [NotifyPropertyChangedFor(nameof(CanCreateRequest))]
    private bool _isArchived;

    public ObservableCollection<UnitDto> Units { get; } = new();
    public ObservableCollection<MaterialGroupDto> Groups { get; } = new();

    public string Header => Id == 0
        ? "Новый материал"
        : $"{Code} — {Name}{(IsArchived ? " (в архиве)" : string.Empty)}";

    public async Task LoadAsync(int id)
    {
        await RunAsync($"Загрузка материала {id}", async () =>
        {
            await LoadReferencesAsync();

            if (id == 0)
            {
                Id = 0;
                Code = string.Empty;
                Name = string.Empty;
                Description = string.Empty;
                SelectedUnit = null;
                SelectedGroup = null;
                IsEditMode = true;
                ShowInfo("Код будет присвоен системой после сохранения.");
                return;
            }

            var material = await _api.GetMaterialAsync(id);

            if (material is null)
            {
                ShowWarning("Материал не найден.");
                return;
            }

            Id = material.Id;
            Code = material.Code;
            Name = material.Name;
            Description = material.Description ?? string.Empty;
            IsArchived = material.IsArchived;
            SelectedUnit = Units.FirstOrDefault(u => u.Id == material.UnitId);
            SelectedGroup = Groups.FirstOrDefault(g => g.Id == material.GroupId);
            IsEditMode = false;
            ShowInfo("Данные перечитаны из базы.");
        });
    }

    private async Task LoadReferencesAsync()
    {
        Units.Clear();
        foreach (var u in await _api.GetUnitsAsync())
            Units.Add(u);

        Groups.Clear();
        foreach (var g in await _api.GetMaterialGroupsAsync())
            Groups.Add(g);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new MaterialEditDto
        {
            Name = Name,
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
            UnitId = SelectedUnit?.Id ?? 0,
            GroupId = SelectedGroup?.Id ?? 0
        };

        var errors = DtoValidator.Validate(dto);
        if (errors.Count > 0)
        {
            ShowWarning(string.Join(Environment.NewLine, errors));
            return;
        }

        await RunAsync("Сохранение материала", async () =>
        {
            if (Id == 0)
            {
                var created = await _api.CreateMaterialAsync(dto);
                Id = created.Id;
                Code = created.Code;
                ShowSuccess($"Материал создан, код {created.Code}.");
            }
            else
            {
                await _api.UpdateMaterialAsync(Id, dto);
                ShowSuccess("Изменения сохранены.");
            }

            IsEditMode = false;
        });
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (Id != 0)
        {
            await LoadAsync(Id);
            return;
        }

        var unitId = SelectedUnit?.Id;
        var groupId = SelectedGroup?.Id;

        await RunAsync("Обновление справочников", async () =>
        {
            await LoadReferencesAsync();
            SelectedUnit = Units.FirstOrDefault(u => u.Id == unitId);
            SelectedGroup = Groups.FirstOrDefault(g => g.Id == groupId);
            ShowSuccess("Справочники обновлены.");
        });
    }

    private bool CanArchive() => Id != 0 && !IsEditMode;
    [RelayCommand(CanExecute = nameof(CanArchive))]
    private async Task ToggleArchiveAsync()
    {
        await RunAsync(IsArchived ? "Восстановление материала" : "Архивирование материала", async () =>
        {
            var result = IsArchived ? await _api.RestoreMaterialAsync(Id) : await _api.ArchiveMaterialAsync(Id);
            IsArchived = result.IsArchived;
            ShowSuccess(IsArchived ? "Материал перемещен в архив." : "Материал восстановлен из архива.");
        });
    }
}
