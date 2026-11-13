using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsClient.Views.Interfaces;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class UsersPage : Page, IRefreshablePage
{
    public UsersViewModel ViewModel { get; }

    public UsersPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<UsersViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    public async Task RefreshAsync()
    {
        await ViewModel.LoadAsync();
    }

    private async void OnUserClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is UserDto user)
            await OpenUserCardAsync(user.Id);
    }

    private async Task OpenUserCardAsync(int id)
    {
        try
        {
            var dialog = new UserCardContentDialog(id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();

            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            ViewModel.ReportError($"Открытие карточки пользователя {id}", ex);
        }
    }
}
