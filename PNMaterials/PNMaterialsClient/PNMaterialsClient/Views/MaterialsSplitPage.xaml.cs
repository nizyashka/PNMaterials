using PNMaterialsClient.ViewModels;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialsSplitPage : Page
{
    public MaterialGroupsViewModel MaterialGroupsViewModel { get; }

    public MaterialsSplitPage()
    {
        this.InitializeComponent();
        MaterialGroupsViewModel = App.Services.GetRequiredService<MaterialGroupsViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await MaterialGroupsViewModel.LoadAsync();
        MaterialsList.ViewModel.ShowSelectGroupHint();
    }

    private async void OnGroupClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialGroupDto group)
        {
            MaterialsList.ViewModel.GroupId = group.Id;
            await MaterialsList.ViewModel.LoadAsync();
        }
    }
}
