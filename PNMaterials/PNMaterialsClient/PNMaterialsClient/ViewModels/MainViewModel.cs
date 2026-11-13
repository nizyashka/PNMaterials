using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsClient.Services;
using static Uno.UI.FeatureConfiguration;

namespace PNMaterialsClient.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly LoginInfoProvider _loginInfoProvider;

    public MainViewModel(ILogger<MainViewModel> logger, AuthService authService, LoginInfoProvider loginInfoProvider) : base(logger)
    {
        _authService = authService;
        _loginInfoProvider = loginInfoProvider;
    }

    public async Task<bool> CanAccess(string requiredRole)
    {
        bool result = false;

        if (string.IsNullOrWhiteSpace(requiredRole))
        {
            result = true;
        }
        else
        {
            await RunAsync("Повторный логин с целью обновления ролей.", async () => 
            { 
                await _authService.Relogin();
                result = _loginInfoProvider.Roles.Contains(requiredRole);

                if (!result)
                    MessagesService.ShowWarning("У вас недостаточно прав для этого действия.");
            });
        }

        return result;
    }
}
