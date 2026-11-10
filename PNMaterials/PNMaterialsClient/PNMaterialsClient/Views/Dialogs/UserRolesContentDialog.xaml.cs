using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsClient.ViewModels;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class UserRolesContentDialog : ContentDialog
{
    private readonly RolesCardViewModel ViewModel;
    private readonly int _id;
    public ICommand ExitCommand { get; }

    public UserRolesContentDialog(int id)
    {
        this.InitializeComponent();

        ViewModel = App.Services.GetRequiredService<RolesCardViewModel>();
        _id = id;
        Opened += OnOpened;
        ExitCommand = new RelayCommand(Hide);
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id);
    }
}
