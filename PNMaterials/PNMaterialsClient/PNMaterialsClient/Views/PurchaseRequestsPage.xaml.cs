using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class PurchaseRequestsPage : Page
{
    public PurchaseRequestsViewModel ViewModel { get; }

    private bool _isCardOpen;

    public PurchaseRequestsPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PurchaseRequestsViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private async void OnCreateClick(object sender, RoutedEventArgs e)
    {
        await OpenCardAsync(0);
    }

    private async void OnRequestClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is PurchaseRequestDto request)
            await OpenCardAsync(request.Id);
    }

    private async Task OpenCardAsync(int id)
    {
        if (_isCardOpen)
            return;

        _isCardOpen = true;

        try
        {
            var dialog = new PurchaseRequestCardContentDialog(id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            ViewModel.ReportError($"Открытие карточки заявки {id}", ex);
        }
        finally
        {
            _isCardOpen = false;
        }
    }
}
