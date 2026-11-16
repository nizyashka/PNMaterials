using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsClient.Services;
using static Uno.UI.FeatureConfiguration;

namespace PNMaterialsClient.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly LoginInfoProvider _loginInfoProvider;

    public MainViewModel(ILogger<MainViewModel> logger, AuthService authService, LoginInfoProvider loginInfoProvider) : base(logger, authService)
    {
        _loginInfoProvider = loginInfoProvider;
    }
}
