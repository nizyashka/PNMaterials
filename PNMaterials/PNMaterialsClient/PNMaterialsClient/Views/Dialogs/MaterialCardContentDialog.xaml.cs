using System.ComponentModel;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class MaterialCardContentDialog : ContentDialog
{
    private readonly int _id;
    public ICommand ExitCommand { get; }

    public MaterialCardViewModel ViewModel { get; }
    public int? RequestedMaterialId { get; private set; }

    public MaterialCardContentDialog(int id)
    {
        this.InitializeComponent();
        _id = id;
        ViewModel = App.Services.GetRequiredService<MaterialCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
        Opened += OnOpened;
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id);
    }

    private void OnCreateRequestClick(object sender, RoutedEventArgs e)
    {
        RequestedMaterialId = ViewModel.Id;
        Hide();
    }
}
