using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views;

public sealed partial class ReportPage : Page
{
    public ReportViewModel ViewModel { get; }

    public ReportPage()
    {
        this.InitializeComponent();
        ViewModel = new ReportViewModel(App.Api);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }
}
