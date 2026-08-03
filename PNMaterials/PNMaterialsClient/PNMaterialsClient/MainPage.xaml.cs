using PNMaterialsClient.Views;

namespace PNMaterialsClient;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        ContentFrame.Navigate(typeof(MaterialGroupsPage));
    }

    private void OnMaterialsClick(object sender, RoutedEventArgs e)
        => ContentFrame.Navigate(typeof(MaterialsPage));

    private void OnMaterialGroupsClick(object sender, RoutedEventArgs e)
        => ContentFrame.Navigate(typeof(MaterialGroupsPage));

    private void OnPurchaseRequestsClick(object sender, RoutedEventArgs e)
        => ContentFrame.Navigate(typeof(PurchaseRequestsPage));

    private void OnReportClick(object sender, RoutedEventArgs e)
        => ContentFrame.Navigate(typeof(ReportPage));
}
