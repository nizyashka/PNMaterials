using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialsSplitPage : Page
{
    public MaterialGroupsViewModel MaterialGroupsViewModel { get; }
    public MaterialsViewModel MaterialsViewModel { get; }

    public MaterialsSplitPage()
    {
        this.InitializeComponent();
        MaterialGroupsViewModel = App.Services.GetRequiredService<MaterialGroupsViewModel>();
        MaterialsViewModel = App.Services.GetRequiredService<MaterialsViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await MaterialGroupsViewModel.LoadAsync();
        MaterialsViewModel.ShowSelectGroupHint();
    }

    //Методы для групп
    private async void OnGroupClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialGroupDto group)
        {
            MaterialsViewModel.GroupId = group.Id;
            await MaterialsViewModel.LoadAsync();
        }
    }

    private async void OnCreateGroupClick(object sender, RoutedEventArgs e)
    {
        await OpenGroupCardAsync(0);
    }

    private async void OnEditGroupClick(object sender, RoutedEventArgs e)
    {
        if (MaterialsViewModel.GroupId is { } groupId)
            await OpenGroupCardAsync(groupId);
    }

    private async Task OpenGroupCardAsync(int id)
    {
        try
        {
            var dialog = new MaterialGroupCardContentDialog(id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
            await MaterialGroupsViewModel.LoadAsync();
            GroupsList.SelectedItem = MaterialGroupsViewModel.Groups.FirstOrDefault(g => g.Id == MaterialsViewModel.GroupId);

            if (dialog.IsDeleted)
                MaterialGroupsViewModel.ShowDeleted();
        }
        catch (Exception ex)
        {
            MaterialGroupsViewModel.ReportError($"Открытие карточки группы {id}", ex);
        }
    }

    //Методы для материалов
    private async void OnCreateMaterialClick(object sender, RoutedEventArgs e)
    {
        await OpenMaterialCardAsync(0);
    }

    private async void OnMaterialClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialDto material)
            await OpenMaterialCardAsync(material.Id);
    }

    private async Task OpenMaterialCardAsync(int id)
    {
        try
        {
            var dialog = new MaterialCardContentDialog(id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();

            if (dialog.RequestedMaterialId is { } requestedMaterialId)
            {
                var requestDialog = new PurchaseRequestCardContentDialog(0, requestedMaterialId)
                {
                    XamlRoot = XamlRoot
                };

                await requestDialog.ShowAsync();
            }

            await MaterialsViewModel.LoadAsync();

            if (dialog.IsDeleted)
                MaterialsViewModel.ShowDeleted();
        }
        catch (Exception ex)
        {
            MaterialsViewModel.ReportError($"Открытие карточки материала {id}", ex);
        }
    }
}
