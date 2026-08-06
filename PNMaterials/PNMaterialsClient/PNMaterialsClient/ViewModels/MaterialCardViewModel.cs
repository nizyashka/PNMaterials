using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialCardViewModel : ObservableObject
{
    private readonly ApiClient _api;

    public MaterialCardViewModel(ApiClient api)
    {
        _api = api;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    [NotifyPropertyChangedFor(nameof(CanCreateRequest))]
    private int _id;

    public bool CanCreateRequest => Id != 0;

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
    private bool _isEditMode;

    [ObservableProperty]
    private string? _status;

    public ObservableCollection<UnitDto> Units { get; } = new();
    public ObservableCollection<MaterialGroupDto> Groups { get; } = new();

    public string Header => Id == 0
        ? "Новый материал"
        : $"{Code} — {Name}";

    public async Task LoadAsync(int id)
    {
        try
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
                Status = "Код будет присвоен системой после сохранения.";
                return;
            }

            var material = await _api.GetMaterialAsync(id);

            if (material is null)
            {
                Status = "Материал не найден.";
                return;
            }

            Id = material.Id;
            Code = material.Code;
            Name = material.Name;
            Description = material.Description ?? string.Empty;
            SelectedUnit = Units.FirstOrDefault(u => u.Id == material.UnitId);
            SelectedGroup = Groups.FirstOrDefault(g => g.Id == material.GroupId);
            IsEditMode = false;
            Status = null;
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    private async Task LoadReferencesAsync()
    {
        if (Units.Count == 0)
        {
            foreach (var u in await _api.GetUnitsAsync())
                Units.Add(u);
        }

        Groups.Clear();
        foreach (var g in await _api.GetMaterialGroupsAsync())
            Groups.Add(g);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedUnit is null)
        {
            Status = "Не выбрана единица измерения.";
            return;
        }

        if (SelectedGroup is null)
        {
            Status = "Не выбрана группа материалов.";
            return;
        }

        try
        {
            var dto = new MaterialEditDto
            {
                Name = Name,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                UnitId = SelectedUnit.Id,
                GroupId = SelectedGroup.Id
            };

            if (Id == 0)
            {
                var created = await _api.CreateMaterialAsync(dto);
                Id = created.Id;
                Code = created.Code;
                Status = $"Материал создан, код {created.Code}.";
            }
            else
            {
                await _api.UpdateMaterialAsync(Id, dto);
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
}
