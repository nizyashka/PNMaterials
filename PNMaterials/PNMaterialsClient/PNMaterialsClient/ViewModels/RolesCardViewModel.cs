using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PNMaterialsClient.Services;
using PNMaterialsContracts;
using Windows.System;

namespace PNMaterialsClient.ViewModels;

public partial class RolesCardViewModel : ViewModelBase
{
    private readonly UsersApiClient _usersApiClient;
    private int _id { get; set; }

    [ObservableProperty]
    private bool _isEditMode = true;

    public ObservableCollection<RoleSelectionViewModel> Roles { get; } = new();

    public RolesCardViewModel(ILogger<RolesCardViewModel> logger, UsersApiClient usersApiClient) : base(logger)
    {
        _usersApiClient = usersApiClient;
    }

    public async Task LoadAsync(int id)
    {
        _id = id;

        await RunAsync("Загрузка ролей.", async () =>
        {
            List<string> allRoles = await _usersApiClient.GetAllRoles();
            UserDto user = await _usersApiClient.GetUserByIdAsync(id);

            Roles.Clear();
            foreach (var role in allRoles)
            {
                Roles.Add(new RoleSelectionViewModel { Name = role, IsSelected = user.Roles.Contains(role) });
            }

            MessagesService.ShowInfo("Данные перечитаны из базы.");
        });
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync(_id);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await RunAsync("Сохранение выбранных ролей.", async () => 
        {
            RolesDto roles = new RolesDto { Roles = Roles.Where(r => r.IsSelected).Select(r => r.Name).ToList() };
            RolesDto newRoles = await _usersApiClient.EditRolesAsync(_id, roles);

            foreach (var role in Roles)
            {
                role.IsSelected = newRoles.Roles.Contains(role.Name);
            }

            IsEditMode = false;

            MessagesService.ShowSuccess("Роли успешно обновлены.");
        });
    }
}
