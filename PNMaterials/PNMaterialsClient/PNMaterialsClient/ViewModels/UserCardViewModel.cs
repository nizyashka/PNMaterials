using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PNMaterialsClient.Services;
using PNMaterialsContracts;
using Windows.System;

namespace PNMaterialsClient.ViewModels;

public partial class UserCardViewModel : ViewModelBase
{
    private readonly UsersApiClient _usersApiClient;
    private readonly LoginInfoProvider _loginInfoProvider;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ArchiveCommand))]
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
    public bool _isBlocked;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ArchiveCommand))]
    public bool _isEditMode;

    [ObservableProperty]
    private AuditInfoDto? _auditInfo;

    public string Header => "Пользователь";

    public ObservableCollection<RoleSelectionViewModel> Roles { get; set; } = new();

    public UserCardViewModel(ILogger<UserCardViewModel> logger, UsersApiClient usersApiClient, LoginInfoProvider loginInfoProvider, AuthService authService) : base(logger, authService)
    {
        _usersApiClient = usersApiClient;
        _loginInfoProvider = loginInfoProvider;
    }

    public async Task<bool> LoadAsync(int id)
    {
        bool loaded = false;
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

                loaded = true;

                return;
            }

            IsEditMode = false;

            UserDto user = await _usersApiClient.GetUserByIdAsync(id);

            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
            MiddleName = user.MiddleName ?? "";
            IsBlocked = user.IsBlocked;
            AuditInfo = user.Audit;

            Password = string.Empty;
            Roles.Clear();

            foreach (var role in allRoles)
            {
                Roles.Add(new RoleSelectionViewModel { Name = role, IsSelected = user.Roles.Contains(role) });
            }

            loaded = true;
        });

        return loaded;
    }

    private bool CanSave() =>
        (!string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(FirstName)
        && !string.IsNullOrWhiteSpace(LastName)
        && (Id == 0 ? !string.IsNullOrWhiteSpace(Password) : true));

    [RelayCommand]
    private void Edit()
    {
        if (IsAuthorized(PNRoles.Admin))
            IsEditMode = true;
    }

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
                Password = string.Empty;

                Id = createdUser.Id;
                Username = createdUser.Username;
                FirstName = createdUser.FirstName;
                LastName = createdUser.LastName;
                MiddleName = createdUser.MiddleName ?? "";
                IsBlocked = createdUser.IsBlocked;
                AuditInfo = createdUser.Audit;

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

                if (Id == _loginInfoProvider.Id)
                {
                    if (!string.IsNullOrWhiteSpace(editedUser.Password))
                        _loginInfoProvider.SetPassword(editedUser.Password);

                    _loginInfoProvider.SetUsername(updatedUser.Username);
                    _loginInfoProvider.SetRoles(updatedUser.Roles);
                }

                IsEditMode = false;
                Password = string.Empty;

                Id = updatedUser.Id;
                Username = updatedUser.Username;
                FirstName = updatedUser.FirstName;
                LastName = updatedUser.LastName;
                MiddleName = updatedUser.MiddleName ?? "";
                IsBlocked= updatedUser.IsBlocked;
                AuditInfo = updatedUser.Audit;

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

    private bool CanArchive() => Id != 0 && !IsEditMode;

    [RelayCommand(CanExecute = nameof(CanArchive))]
    public async Task ArchiveAsync()
    {
        await RunAsync(IsBlocked ? "Снятие блокировки пользователя." : "Блокировка пользователя", async () =>
        {
            UserDto user;

            if (IsBlocked)
                user = await _usersApiClient.UnblockUserAsync(Id);
            else
                user = await _usersApiClient.BlockUserAsync(Id);

            IsBlocked = user.IsBlocked;
            AuditInfo = user.Audit;

            MessagesService.ShowSuccess(IsBlocked ? "Пользователь успешно заблокирован." : "Пользователь успешно разблокирован.");
        });
    }
}
