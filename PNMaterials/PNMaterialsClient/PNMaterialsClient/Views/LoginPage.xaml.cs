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
        LoginViewModel.Bind(OnSuccessfulLogin, OnFailedLogin);
        FailedLoginTextBlock.Visibility = Visibility.Collapsed;
    }

    private void OnSuccessfulLogin()
    {
        FailedLoginTextBlock.Visibility = Visibility.Collapsed;
        if (Frame.Navigate(typeof(MainPage)))
        {
            Frame.BackStack.Clear();
            Frame.ForwardStack.Clear();
        }
    }

    private void OnFailedLogin()
    {
        FailedLoginTextBlock.Visibility = Visibility.Visible;
    }
}
