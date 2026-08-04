using PNMaterialsClient.ViewModels;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialsPage : Page
{
    public MaterialsViewModel ViewModel { get; }

    public MaterialsPage()
    {
        this.InitializeComponent();
        ViewModel = new MaterialsViewModel(App.Api);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private void OnCreateClick(object sender, RoutedEventArgs e)
    => Frame.Navigate(typeof(MaterialCardPage), 0);

    private void OnMaterialClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialDto material)
            Frame.Navigate(typeof(MaterialCardPage), material.Id);
    }
}
