using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public class UsersViewModel : ViewModelBase
{
    private readonly UsersApiClient _usersApiClient;

    public ObservableCollection<UserDto> Users { get; } = new();

    public UsersViewModel(UsersApiClient usersApiClient, ILogger<UsersViewModel> logger, AuthService authService) : base(logger, authService)
    {
        _usersApiClient = usersApiClient;
    }

    public async Task LoadAsync()
    {
        await RunAsync("Загрузка всех пользователей.", async () =>
        {
            var users = await _usersApiClient.GetAllUsersAsync();

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        });
    }
}
