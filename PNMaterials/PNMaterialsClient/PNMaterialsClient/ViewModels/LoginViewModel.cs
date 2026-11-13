using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private AuthService _authService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LogInCommand))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LogInCommand))]
    private string _password = string.Empty;

    private Action? OnSuccessfulLogin { get; set; }
    private Action? OnFailedLogin { get; set; }

    public LoginViewModel(AuthService authService, ILogger<LoginViewModel> logger) : base(logger)
    {
        _authService = authService;
    }

    [RelayCommand(CanExecute = nameof(TextBoxesAreFilled))]
    public async Task LogInAsync()
    {
        var onSuccessfulLogin = OnSuccessfulLogin 
            ?? throw new InvalidOperationException("Обработчик успешного входа не задан.");

        var onFailedLogin = OnFailedLogin
            ?? throw new InvalidOperationException("Обработчик неудачного входа не задан.");

        bool loggedIn = false;

        await RunAsync("Попытка аутентификации.", async () =>
        {
            await _authService.Login(Username!, Password!);
            loggedIn = true;
            onSuccessfulLogin();
        });

        if (!loggedIn)
            onFailedLogin();
    }

    public void Bind(Action onSuccessfulLogin, Action onFailedLogin)
    {
        OnSuccessfulLogin = onSuccessfulLogin;
        OnFailedLogin = onFailedLogin;
    }

    private bool TextBoxesAreFilled() => !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Username);
}
