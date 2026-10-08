using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialGroupsPage : Page
{
    public MaterialGroupsViewModel ViewModel { get; }

    public MaterialGroupsPage()
    {
        this.InitializeComponent();
        ViewModel = new MaterialGroupsViewModel(App.Api);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private async void OnCreateClick(object sender, RoutedEventArgs e)
    {
        MaterialGroupCardContentDialog groupCardDialog = new MaterialGroupCardContentDialog(0)
        {
            XamlRoot = this.XamlRoot
        };
        await groupCardDialog.ShowAsync();
        await ViewModel.LoadAsync();
    }

    private async void OnGroupClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialGroupDto group)
        {
            MaterialGroupCardContentDialog groupCardDialog = new MaterialGroupCardContentDialog(group.Id)
            {
                XamlRoot = this.XamlRoot
            };
            await groupCardDialog.ShowAsync();
            await ViewModel.LoadAsync();
        }
    }
}
