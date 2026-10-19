using PNMaterialsClient.ViewModels;
using PNMaterialsContracts;
using PNMaterialsClient.Views.Dialogs;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialsPage : Page
{
    public MaterialsViewModel ViewModel { get; }

    private bool _isCardOpen;

    public MaterialsPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MaterialsViewModel>();
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

    private async void OnMaterialClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialDto material)
            await OpenCardAsync(material.Id);
    }

    private async Task OpenCardAsync(int id)
    {
        if (_isCardOpen)
            return;

        _isCardOpen = true;

        try
        {
            var dialog = new MaterialCardContentDialog(id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();

            if (dialog.RequestedMaterialId is { } requestedMaterialId)
            {
                var requestDialog = new PurchaseRequestCardContentDialog(0, requestedMaterialId)
                {
                    XamlRoot = XamlRoot
                };

                await requestDialog.ShowAsync();
            }

            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            ViewModel.ReportError($"Открытие карточки материала {id}", ex);
        }
        finally
        {
            _isCardOpen = false;
        }
    }
}
