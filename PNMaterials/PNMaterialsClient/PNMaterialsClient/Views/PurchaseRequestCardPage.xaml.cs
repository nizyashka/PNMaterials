using System.Collections.ObjectModel;
using PNMaterialsClient.ViewModels;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class PurchaseRequestCardPage : Page
{
    public PurchaseRequestCardViewModel ViewModel { get; }
    public ICommand ExitCommand { get; }

    public PurchaseRequestCardPage()
    {
        this.InitializeComponent();

        ViewModel = new PurchaseRequestCardViewModel(App.Api);
        ExitCommand = new RelayCommand(() =>
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        });
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var id = e.Parameter is int value ? value : 0;
        await ViewModel.LoadAsync(id);
    }
}
