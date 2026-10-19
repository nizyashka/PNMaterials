using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class PurchaseRequestCardContentDialog : ContentDialog
{
    private readonly int _id;
    private readonly int? _materialId;

    public PurchaseRequestCardViewModel ViewModel { get; }
    public ICommand ExitCommand { get; }

    public PurchaseRequestCardContentDialog(int id, int? materialId = null)
    {
        this.InitializeComponent();
        _id = id;
        _materialId = materialId;
        ViewModel = App.Services.GetRequiredService<PurchaseRequestCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
        Opened += OnOpened;
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id, _materialId);
    }
}
