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

    private async void OnEditUserProfileClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem button && button.DataContext is UserDto user)
        {
            await OpenUserProfileEditDialog(user);
        }
    }

    private async void OnEditUserRolesClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem button && button.DataContext is UserDto user)
            await OpenUserRolesEditDialog(user);
    }

    private async void OnEditUserPasswordClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem button && button.DataContext is UserDto user)
            await OpenUserPasswordEditDialog(user);
    }

    private async Task OpenUserProfileEditDialog(UserDto dto)
    {
        try
        {
            var dialog = new UserProfileContentDialog(dto.Id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            ViewModel.ReportError($"Редактирование профиля пользователя {dto.Username}", ex);
        }
    }

    private async Task OpenUserRolesEditDialog(UserDto dto)
    {
        try
        {
            var dialog = new UserRolesContentDialog(dto.Id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            ViewModel.ReportError($"Изменение ролей пользователя", ex);
        }
    }

    private async Task OpenUserPasswordEditDialog(UserDto dto)
    {
        try
        {
            var dialog = new UserPasswordContentDialog(dto.Id)
            {
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            ViewModel.ReportError($"Изменение пароля пользователя", ex);
        }
    }

    public async Task RefreshAsync()
    {
        await ViewModel.LoadAsync();
    }

    private async void OnUserClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is UserDto user)
            await OpenMaterialCardAsync(user.Id);
    }

    private async Task OpenMaterialCardAsync(int id)
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
