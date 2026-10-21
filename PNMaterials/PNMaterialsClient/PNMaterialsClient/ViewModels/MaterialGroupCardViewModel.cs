using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialGroupCardViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    public MaterialGroupCardViewModel(ApiClient api, ILogger<MaterialGroupCardViewModel> logger)
        : base(logger)
    {
        _api = api;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    private int _id;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Header))]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _isEditMode;

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
            ShowInfo("Заполните поля и нажмите «Сохранить».");
            return;
        }

        await RunAsync($"Загрузка группы {id}", async () =>
        {
            var group = await _api.GetMaterialGroupAsync(id);

            if (group is null)
            {
                ShowWarning("Группа не найдена.");
                return;
            }

            Id = group.Id;
            Name = group.Name;
            Description = group.Description ?? string.Empty;
            IsEditMode = false;
            ShowInfo("Данные перечитаны из базы.");
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new MaterialGroupEditDto
        {
            Name = Name,
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description
        };

        var errors = DtoValidator.Validate(dto);
        if (errors.Count > 0)
        {
            ShowWarning(string.Join(Environment.NewLine, errors));
            return;
        }

        await RunAsync("Сохранение группы", async () =>
        {
            if (Id == 0)
            {
                var created = await _api.CreateMaterialGroupAsync(dto);
                Id = created.Id;
                ShowSuccess("Группа создана.");
            }
            else
            {
                await _api.UpdateMaterialGroupAsync(Id, dto);
                ShowSuccess("Изменения сохранены.");
            }

            IsEditMode = false;
        });
    }

    private bool CanRefresh() => Id != 0;

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    private async Task RefreshAsync()
    {
        await LoadAsync(Id);
    }

    public async Task<bool> DeleteAsync()
    {
        bool deleted = false;

        await RunAsync("Удаление группы", async () =>
        {
            await _api.DeleteMaterialGroupAsync(Id);
            deleted = true;
        });

        return deleted;
    }
}
