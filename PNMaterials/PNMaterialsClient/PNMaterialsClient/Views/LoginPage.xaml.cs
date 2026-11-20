using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Input;
using PNMaterialsClient.ViewModels;
using Windows.System;

namespace PNMaterialsClient.Views;

public sealed partial class LoginPage : Page
{
    private readonly LoginViewModel LoginViewModel;

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

    private void OnLoginTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter)
            return;

        e.Handled = true;
        PasswordTextBox.Focus(FocusState.Keyboard);
    }

    private void OnPasswordTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter)
            return;

        e.Handled = true;

        var command = LoginViewModel.LogInCommand;
        if (command.CanExecute(null))
            command.Execute(null);
    }
}
