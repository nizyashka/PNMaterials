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
    public int _id;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public string _username;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public string _firstName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public string _lastName;

    [ObservableProperty]
    public string _middleName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
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
        Id = id;

        await RunAsync($"Загрузка пользователя {id}", async () =>
        {
            List<string> allRoles = await _usersApiClient.GetAllRoles();

            if (id == 0)
            {
                IsEditMode = true;
                Roles.Clear();

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

            Roles.Clear();

            foreach (var role in allRoles)
            {
                Roles.Add(new RoleSelectionViewModel { Name = role, IsSelected = user.Roles.Contains(role) });
            }

            MessagesService.ShowInfo("Данные о пользователе загружены из базы.");
        });
    }

    private bool CanSave() =>
        (!string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(FirstName)
        && !string.IsNullOrWhiteSpace(LastName)
        && (Id == 0 ? !string.IsNullOrWhiteSpace(Password) : true));

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (Id == 0)
        {
            CreateUserDto newUser = new CreateUserDto
            {
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                MiddleName = MiddleName,
                Password = Password,
                Roles = Roles.Where(r => r.IsSelected).Select(r => r.Name).ToList()
            };

            await RunAsync("Создание нового пользователя.", async () => 
            { 
                UserDto createdUser = await _usersApiClient.CreateUserAsync(newUser);

                MessagesService.ShowSuccess("Пользователь успешно создан.");

                IsEditMode = false;

                Id = createdUser.Id;
                Username = createdUser.Username;
                FirstName = createdUser.FirstName;
                LastName = createdUser.LastName;
                MiddleName = createdUser.MiddleName ?? "";

                foreach (var role in Roles)
                {
                    role.IsSelected = createdUser.Roles.Contains(role.Name);
                }
            });
        }
        else
        {
            UserEditDto editedUser = new UserEditDto
            {
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                MiddleName = MiddleName,
                Password = Password,
                Roles = Roles.Where(r => r.IsSelected).Select(r => r.Name).ToList()
            };

            await RunAsync("Изменение пользователя.", async () =>
            {
                UserDto updatedUser = await _usersApiClient.EditUserAsync(Id, editedUser);

                MessagesService.ShowSuccess("Пользователь успешно изменен.");

                IsEditMode = false;

                Id = updatedUser.Id;
                Username = updatedUser.Username;
                FirstName = updatedUser.FirstName;
                LastName = updatedUser.LastName;
                MiddleName = updatedUser.MiddleName ?? "";

                foreach (var role in Roles)
                {
                    role.IsSelected = updatedUser.Roles.Contains(role.Name);
                }
            });
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (Id != 0)
        {
            await LoadAsync(Id);
            return;
        }

        var selectedRoles = Roles.Where(r => r.IsSelected).Select(r => r.Name).ToList();

        await RunAsync("Обновление ролей.", async () =>
        {
            List<string> allRoles = await _usersApiClient.GetAllRoles();

            Roles.Clear();

            foreach (string role in allRoles)
            {
                Roles.Add(new RoleSelectionViewModel { Name = role, IsSelected = selectedRoles.Contains(role) });
            }

            MessagesService.ShowSuccess("Список ролей успешно обновлен.");
        });
    }
}
