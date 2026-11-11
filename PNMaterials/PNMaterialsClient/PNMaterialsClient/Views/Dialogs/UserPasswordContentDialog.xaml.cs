using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class UserPasswordContentDialog : ContentDialog
{
    public ICommand ExitCommand { get; }

    private PasswordCardViewModel ViewModel { get; }

    public UserPasswordContentDialog(int userId)
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PasswordCardViewModel>();
        ViewModel.UserId = userId;
        ViewModel.OnSuccess = Hide;
        ExitCommand = new RelayCommand(Hide);
    }
}
