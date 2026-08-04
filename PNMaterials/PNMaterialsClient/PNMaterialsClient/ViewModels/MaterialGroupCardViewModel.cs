using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialGroupCardViewModel : ObservableObject
{
    private readonly ApiClient _api;

    public MaterialGroupCardViewModel(ApiClient api)
    {
        _api = api;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    private int _id;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string? _status;

    public string Header => Id == 0
        ? "Новая группа материалов"
        : $"{Id} — {Name}";

    public async Task LoadAsync(int id)
    {
        if (id == 0)
        {
            Id = 0;
            Name = string.Empty;
            Description = string.Empty;
            IsEditMode = true;
            Status = "Заполните поля и нажмите «Сохранить».";
            return;
        }

        try
        {
            var group = await _api.GetMaterialGroupAsync(id);

            if (group is null)
            {
                Status = "Группа не найдена.";
                return;
            }

            Id = group.Id;
            Name = group.Name;
            Description = group.Description ?? string.Empty;
            IsEditMode = false;
            Status = null;
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            var dto = new MaterialGroupEditDto
            {
                Name = Name,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description
            };

            if (Id == 0)
            {
                var created = await _api.CreateMaterialGroupAsync(dto);
                Id = created.Id;
                Status = "Группа создана.";
            }
            else
            {
                await _api.UpdateMaterialGroupAsync(Id, dto);
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
