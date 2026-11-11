using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class UserCardViewModel : ViewModelBase
{
    private readonly UsersApiClient _usersApiClient;

    [ObservableProperty]
    public string _username;

    [ObservableProperty]
    public string _firstName;

    [ObservableProperty]
    public string _lastName;

    [ObservableProperty]
    public string _middleName;

    [ObservableProperty]
    public string _password;

    [ObservableProperty]
    public bool _isEditMode;

    public string Header => "Пользователь";

    public ObservableCollection<RoleSelectionViewModel> Roles { get; set; } = new();

    public UserCardViewModel(ILogger<UserCardViewModel> logger, UsersApiClient usersApiClient) : base(logger)
    {
        _usersApiClient = usersApiClient;
    }

    public async Task LoadAsync(int id)
    {
        await RunAsync($"Загрузка пользователя {id}", async () => 
        {
            List<string> allRoles = await _usersApiClient.GetAllRoles();

            if (id == 0)
            {
                IsEditMode = true;
                foreach (string role in allRoles)
                {
                    Roles.Add(new RoleSelectionViewModel { Name = role, IsSelected = false });
                }

                return;
            }

            IsEditMode = false;

            UserDto user = await _usersApiClient.GetUserByIdAsync(id);

            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
            MiddleName = user.MiddleName ?? "";

            foreach (var role in allRoles)
            {
                Roles.Add(new RoleSelectionViewModel { Name = role, IsSelected = user.Roles.Contains(role) });
            }

            MessagesService.ShowInfo("Данные о пользователе загружены из базы.");
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {

    }

    [RelayCommand]
    private async Task RefreshAsync()
    {

    }
}
