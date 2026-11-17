using Microsoft.UI.Xaml.Controls.Primitives;
using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsClient.Views.Interfaces;
using PNMaterialsContracts;
using WinUI.TableView;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialsSplitPage : Page, IRefreshablePage
{
    public MaterialGroupsViewModel MaterialGroupsViewModel { get; }
    public MaterialsViewModel MaterialsViewModel { get; }

    public Predicate<TableViewConditionalCellStyleContext> IsArchivedMaterial { get; } =
        context => context.DataItem is MaterialDto { IsArchived: true };

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

    private async void OnEditGroupClick(object sender, RoutedEventArgs e)
    {
        if (!MaterialGroupsViewModel.IsAuthorized(PNRoles.ReadAndWrite))
            return;

        if (sender is Button button && button.DataContext is MaterialGroupDto group)
            await OpenGroupCardAsync(group.Id);
    }

    private async void OnIncludeArchivedClick(object sender, RoutedEventArgs e)
    {
        var includeArchived = ((ToggleButton)sender).IsChecked == true;

        ArchiveToggleIcon.Glyph = includeArchived ? "\uED1A" : "\uE890";

        MaterialsViewModel.IncludeArchived = includeArchived;
        await MaterialsViewModel.LoadAsync();
    }

    private async Task OpenGroupCardAsync(int id)
    {
        try
        {
            var dialog = new MaterialGroupCardContentDialog()
            {
                XamlRoot = XamlRoot
            };

            if (!await dialog.ViewModel.LoadAsync(id))
                return;

            await dialog.ShowAsync();
            await MaterialGroupsViewModel.LoadAsync();
            GroupsList.SelectedItem = MaterialGroupsViewModel.Groups.FirstOrDefault(g => g.Id == MaterialsViewModel.GroupId);
        }
        catch (Exception ex)
        {
            MaterialGroupsViewModel.ReportError($"Открытие карточки группы {id}", ex);
        }
    }

    //Методы для материалов
    private async void OnMaterialClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialDto material)
            await OpenMaterialCardAsync(material.Id);
    }

    private async Task OpenMaterialCardAsync(int id)
    {
        try
        {
            var dialog = new MaterialCardContentDialog()
            {
                XamlRoot = XamlRoot
            };

            if (!await dialog.ViewModel.LoadAsync(id))
                return;

            await dialog.ShowAsync();

            if (dialog.RequestedMaterialId is { } requestedMaterialId)
            {
                var requestDialog = new PurchaseRequestCardContentDialog()
                {
                    XamlRoot = XamlRoot
                };

                if (await requestDialog.ViewModel.LoadAsync(0, requestedMaterialId))
                    await requestDialog.ShowAsync();
            }

            await MaterialsViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            MaterialsViewModel.ReportError($"Открытие карточки материала {id}", ex);
        }
    }

    public async Task RefreshAsync()
    {
        await MaterialGroupsViewModel.LoadAsync();

        GroupsList.SelectedItem = MaterialGroupsViewModel.Groups
            .FirstOrDefault(g => g.Id == MaterialsViewModel.GroupId);

        await MaterialsViewModel.LoadAsync();
    }
}
