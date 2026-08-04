using PNMaterialsClient.ViewModels;
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

    private void OnCreateClick(object sender, RoutedEventArgs e)
        => Frame.Navigate(typeof(MaterialGroupCardPage), 0);

    private void OnGroupClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialGroupDto group)
            Frame.Navigate(typeof(MaterialGroupCardPage), group.Id);
    }
}
