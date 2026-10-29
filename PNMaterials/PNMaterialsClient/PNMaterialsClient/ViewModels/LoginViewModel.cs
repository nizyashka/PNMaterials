using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private AuthApiClient _authApiClient;
    private AccessTokenProvider _accessTokenProvider;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LogInCommand))]
    public string _login;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LogInCommand))]
    public string _password;

    private Action OnSuccessfulLogin { get; set; }
    private Action OnFailedLogin { get; set; }

    public LoginViewModel(
        AuthApiClient authApiClient, 
        AccessTokenProvider accessTokenProvider, 
        ILogger<MaterialCardViewModel> logger) 
        : base(logger)
    {
        _authApiClient = authApiClient;
        _accessTokenProvider = accessTokenProvider;
    }

    [RelayCommand(CanExecute = nameof(TextBoxesAreFilled))]
    public async Task LogInAsync()
    {
        bool loggedIn = false;

        await RunAsync("Попытка входа.", async () =>
        {
            var result = await _authApiClient.Login(new LoginDto { Login = Login!, Password = Password! });
            //Че-то делать с результатом
            _accessTokenProvider.SetAccessToken(result.Token);
            loggedIn = true;
            OnSuccessfulLogin();
        });

        if (!loggedIn)
            OnFailedLogin();
    }

    public void Bind(Action onSuccessfulLogin, Action onFailedLogin)
    {
        OnSuccessfulLogin = onSuccessfulLogin;
        OnFailedLogin = onFailedLogin;
    }

    private bool TextBoxesAreFilled() => !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Login);
}
