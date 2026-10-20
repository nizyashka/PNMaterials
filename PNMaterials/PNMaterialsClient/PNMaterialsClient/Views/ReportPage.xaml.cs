using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class ReportPage : Page
{
    public ReportViewModel ReportViewModel { get; }

    public ReportPage()
    {
        this.InitializeComponent();
        ReportViewModel = App.Services.GetRequiredService<ReportViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ReportViewModel.LoadAsync();
    }

    private void OnStatusesFlyoutClosed(object sender, object e)
        => ReportViewModel.RefreshStatusesSummary();

    private async void OnRequestClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is PurchaseRequestReportRowDto request)
            await OpenCardAsync(request.Id);
    }

    private async Task OpenCardAsync(int id)
    {
        try
        {
            var dialog = new PurchaseRequestCardContentDialog(id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
            await ReportViewModel.SearchCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            ReportViewModel.ReportError($"Открытие карточки заявки {id}", ex);
        }
    }
}
