using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class PurchaseRequestCardContentDialog : ContentDialog
{
    private readonly int _id;

    public PurchaseRequestCardViewModel ViewModel { get; }
    public ICommand ExitCommand { get; }

    public PurchaseRequestCardContentDialog(int id)
    {
        this.InitializeComponent();
        _id = id;
        ViewModel = new PurchaseRequestCardViewModel(App.Api);
        ExitCommand = new RelayCommand(Hide);
        Opened += OnOpened;
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id);
    }
}
