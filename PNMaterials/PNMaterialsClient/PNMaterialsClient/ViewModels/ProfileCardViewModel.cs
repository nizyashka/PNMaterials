using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class ProfileCardViewModel : ViewModelBase
{
    private readonly UsersApiClient _usersApiClient;
    private LoginInfoProvider _loginInfoProvider;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public int _userId;

    [ObservableProperty]
    public string _username;

    [ObservableProperty]
    public string _firstName;

    [ObservableProperty]
    public string _lastName;

    [ObservableProperty]
    public string? _middleName;

    [ObservableProperty]
    public bool _isEditMode = true;

    public ProfileCardViewModel(ILogger<ProfileCardViewModel> logger, UsersApiClient usersApiClient, LoginInfoProvider loginInfoProvider) : base(logger)
    {
        _usersApiClient = usersApiClient;
        _loginInfoProvider = loginInfoProvider;
    }

    public async Task LoadAsync(int id)
    {
        UserId = id;

        await RunAsync($"Загрузка профиля пользователя с ID: {id}.", async () =>
        {
            UserDto user = await _usersApiClient.GetUserByIdAsync(id);

            UserId = user.Id;
            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
            MiddleName = user.MiddleName;

            MessagesService.ShowInfo("Загружен профиль пользователя.");
        });
    }

    private bool HasId() => UserId > 0;

    [RelayCommand(CanExecute = nameof(HasId))]
    private async Task SaveAsync()
    {
        await RunAsync("Сохранение изменений профиля.", async () =>
        {
            ProfileEditDto newProfile = new ProfileEditDto
            {
                Username = this.Username,
                FirstName = this.FirstName,
                LastName = this.LastName,
                MiddleName = this.MiddleName
            };

            ProfileDto result = await _usersApiClient.EditProfileAsync(UserId, newProfile);

            Username = result.Username;
            FirstName = result.FirstName;
            LastName = result.LastName;
            MiddleName = result.MiddleName;

            if (_loginInfoProvider.Id == UserId)
                _loginInfoProvider.SetUsername(Username);

            IsEditMode = false;

            MessagesService.ShowSuccess("Данные успешно изменены.");
        });
    }

    [RelayCommand(CanExecute = nameof(HasId))]
    private async Task RefreshAsync()
    {
        await LoadAsync(UserId);
    }
}
