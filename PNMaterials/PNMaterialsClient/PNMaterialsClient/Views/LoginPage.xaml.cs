using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views;

public sealed partial class LoginPage : Page
{
    private LoginViewModel LoginViewModel;

    public LoginPage()
    {
        this.InitializeComponent();
        LoginViewModel = App.Services.GetRequiredService<LoginViewModel>();
        LoginViewModel.Bind(OnSuccessfulLogin);
    }

    private void OnSuccessfulLogin()
    {
        if (Frame.Navigate(typeof(MainPage)))
        {
            Frame.BackStack.Clear();
            Frame.ForwardStack.Clear();
        }
    }
}
