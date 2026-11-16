using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LogInCommand))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LogInCommand))]
    private string _password = string.Empty;

    private Action? OnSuccessfulLogin { get; set; }

    public LoginViewModel(AuthService authService, ILogger<LoginViewModel> logger) : base(logger, authService)
    {
        
    }

    [RelayCommand(CanExecute = nameof(TextBoxesAreFilled))]
    public async Task LogInAsync()
    {
        var onSuccessfulLogin = OnSuccessfulLogin 
            ?? throw new InvalidOperationException("Обработчик успешного входа не задан.");

        await RunAsync("Попытка аутентификации.", async () =>
        {
            await _authService.Login(Username!, Password!);
            onSuccessfulLogin();
        });
    }

    public void Bind(Action onSuccessfulLogin)
    {
        OnSuccessfulLogin = onSuccessfulLogin;
    }

    private bool TextBoxesAreFilled() => !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Username);
}
