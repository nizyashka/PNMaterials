using PNMaterialsClient.ViewModels;
using PNMaterialsContracts;
using PNMaterialsClient.Views.Dialogs;

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

    private async void OnCreateClick(object sender, RoutedEventArgs e)
    {
        MaterialCardContentDialog materialCardDialog = new MaterialCardContentDialog(0)
        {
            XamlRoot = this.XamlRoot
        };
        await materialCardDialog.ShowAsync();
        await ViewModel.LoadAsync();
    }

    private async void OnMaterialClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialDto material)
        {
            MaterialCardContentDialog materialCardDialog = new MaterialCardContentDialog(material.Id)
            {
                XamlRoot = this.XamlRoot
            };
            await materialCardDialog.ShowAsync();
            await ViewModel.LoadAsync();
        }
    }
}
