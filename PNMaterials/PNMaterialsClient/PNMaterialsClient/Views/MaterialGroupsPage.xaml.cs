using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialGroupsPage : Page
{
    public MaterialGroupsViewModel ViewModel { get; }

    private bool _isCardOpen;

    public MaterialGroupsPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MaterialGroupsViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private async void OnCreateClick(object sender, RoutedEventArgs e)
    {
        await OpenCardAsync(0);
    }

    private async void OnGroupClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is MaterialGroupDto group)
            await OpenCardAsync(group.Id);
    }

    private async Task OpenCardAsync(int id)
    {
        if (_isCardOpen)
            return;

        _isCardOpen = true;

        try
        {
            var dialog = new MaterialGroupCardContentDialog(id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            ViewModel.ReportError($"Открытие карточки группы {id}", ex);
        }
        finally
        {
            _isCardOpen = false;
        }
    }
}
