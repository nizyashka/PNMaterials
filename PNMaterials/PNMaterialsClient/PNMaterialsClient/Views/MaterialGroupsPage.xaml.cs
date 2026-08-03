using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialGroupsPage : Page
{
    public MaterialGroupsViewModel ViewModel { get; }

    public MaterialGroupsPage()
    {
        this.InitializeComponent();
        ViewModel = new MaterialGroupsViewModel(App.Api);
        Loaded += async (_, _) => await ViewModel.LoadAsync();
    }
}
