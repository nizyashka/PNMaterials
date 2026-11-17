using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class PurchaseRequestCardContentDialog : ContentDialog
{
    public PurchaseRequestCardViewModel ViewModel { get; }
    public ICommand ExitCommand { get; }

    public PurchaseRequestCardContentDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PurchaseRequestCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
    }
}
