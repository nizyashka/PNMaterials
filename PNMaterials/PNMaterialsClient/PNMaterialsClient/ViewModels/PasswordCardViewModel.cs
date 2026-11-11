using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class PasswordCardViewModel : ViewModelBase
{
    private readonly UsersApiClient _usersApiClient;
    private readonly LoginInfoProvider _loginInfoProvider;

    [ObservableProperty]
    private bool _isEditMode = true;
    public string NewPassword = string.Empty;

    public int UserId { get; set; }

    public Action? OnSuccess { get; set; }

    public PasswordCardViewModel(ILogger<PasswordCardViewModel> logger, UsersApiClient usersApiClient, LoginInfoProvider loginInfoProvider) : base(logger)
    {
        _usersApiClient = usersApiClient;
        _loginInfoProvider = loginInfoProvider;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await RunAsync("Изменение пароля.", async () => 
        {
            await _usersApiClient.EditPasswordAsync(UserId, new PasswordEditDto { NewPassword = NewPassword });

            if (UserId == _loginInfoProvider.Id)
                _loginInfoProvider.SetPassword(NewPassword);

            NewPassword = "";
            IsEditMode = false;
            MessagesService.ShowSuccess("Пароль успешно изменен.");
            OnSuccess?.Invoke();
        });
    }
}
