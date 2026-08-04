using PNMaterialsClient.ViewModels;
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

    private void OnCreateClick(object sender, RoutedEventArgs e)
        => Frame.Navigate(typeof(PurchaseRequestCardPage), 0);

    private void OnRequestClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is PurchaseRequestDto request)
            Frame.Navigate(typeof(PurchaseRequestCardPage), request.Id);
    }
}
