using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class PurchaseRequestsPage : Page
{
    public PurchaseRequestsViewModel ViewModel { get; }

    public PurchaseRequestsPage()
    {
        this.InitializeComponent();
        ViewModel = new PurchaseRequestsViewModel(App.Api);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private async void OnCreateClick(object sender, RoutedEventArgs e)
    {
        PurchaseRequestCardContentDialog requestCardDialog = new PurchaseRequestCardContentDialog(0)
        {
            XamlRoot = this.XamlRoot
        };
        await requestCardDialog.ShowAsync();
        await ViewModel.LoadAsync();
    }

    private async void OnRequestClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is PurchaseRequestDto request)
        {
            PurchaseRequestCardContentDialog requestCardDialog = new PurchaseRequestCardContentDialog(request.Id)
            {
                XamlRoot = this.XamlRoot
            };
            await requestCardDialog.ShowAsync();
            await ViewModel.LoadAsync();
        }
    }
}
