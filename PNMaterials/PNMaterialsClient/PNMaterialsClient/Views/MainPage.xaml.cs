using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.UI;
using PNMaterialsClient.Navigation;
using PNMaterialsClient.Services;
using PNMaterialsClient.ViewModels;
using PNMaterialsClient.Views.Dialogs;
using PNMaterialsClient.Views.Interfaces;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views;

public sealed partial class MainPage : Page
{
    public List<NavItem> NavItems { get; } =
    [
        new NavFolder
        {
            Title = "Справочники",
            Children =
            [
                new NavPage { Title = "Группы и материалы", PageType = typeof(MaterialsSplitPage) },
            ]
        },
        new NavFolder
        {
            Title = "Отчеты",
            Children =
            [
                new NavPage { Title = "Отчет по заявкам на закупку", PageType = typeof(ReportPage) },
            ]
        },
        new NavPage { Title = "Пользователи", PageType = typeof(UsersPage), RequiredRole = PNRoles.Admin }
    ];

    private readonly Dictionary<Type, Button> _sectionByPage = [];
    private readonly MainViewModel ViewModel;

    public MainPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MainViewModel>();
        BuildMenu();
        ContentFrame.Navigated += OnContentFrameNavigated;
        ContentFrame.Navigate(typeof(MaterialsSplitPage));
    }

    private void BuildMenu()
    {
        foreach (var rootItem in NavItems)
        {
            var menuButton = new Button
            {
                Content = rootItem.Title
            };

            if (rootItem is NavFolder parentItem)
            {
                var menuFlyout = new MenuFlyout();

                foreach (var page in parentItem.Children.OfType<NavPage>())
                {
                    var flyoutItem = new MenuFlyoutItem
                    {
                        Text = page.Title,
                        Tag = page
                    };

                    flyoutItem.Click += OnMenuItemClick;
                    menuFlyout.Items.Add(flyoutItem);
                    _sectionByPage[page.PageType] = menuButton;
                }

                menuButton.Flyout = menuFlyout;
            }

            if (rootItem is NavPage pageItem)
            {
                menuButton.Tag = pageItem;
                menuButton.Click += OnMenuItemClick;
                _sectionByPage[pageItem.PageType] = menuButton;
            }

            MainMenu.Items.Add(menuButton);
        }
    }

    private async void OnMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: NavPage page } && ContentFrame.CurrentSourcePageType != page.PageType)
        {
            if (!await ViewModel.CanAccess(page.RequiredRole))
                return;

            ContentFrame.Navigate(page.PageType);
        }
    }

    private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
    {
        ContentFrame.BackStack.Clear();
        ContentFrame.ForwardStack.Clear();

        if (_sectionByPage.TryGetValue(e.SourcePageType, out var section))
            HighlightSection(section);
    }

    private void HighlightSection(Button active)
    {
        var accent = (Brush)Application.Current.Resources["AccentBrush"];

        foreach (var item in MainMenu.Items.OfType<Button>())
        {
            var isActive = item == active;
            item.BorderBrush = isActive ? accent : new SolidColorBrush(Colors.Transparent);
        }
    }

    private async void OnCreateRequestClick(object sender, RoutedEventArgs e)
    {
        var dialog = new PurchaseRequestCardContentDialog(0)
        {
            XamlRoot = XamlRoot
        };

        await dialog.ShowAsync();

        if (ContentFrame.Content is IRefreshablePage page)
            await page.RefreshAsync();
    }

    private async void OnCreateGroupClick(object sender, RoutedEventArgs e)
    {
        var dialog = new MaterialGroupCardContentDialog(0)
        {
            XamlRoot = XamlRoot
        };

        await dialog.ShowAsync();

        if (ContentFrame.Content is IRefreshablePage page)
            await page.RefreshAsync();
    }

    private async void OnCreateMaterialClick(object sender, RoutedEventArgs e)
    {
        var dialog = new MaterialCardContentDialog(0)
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

        if (ContentFrame.Content is IRefreshablePage page)
            await page.RefreshAsync();
    }

    private async void OnCreateUserClick(object sender, RoutedEventArgs e)
    {
        if (!await ViewModel.CanAccess(PNRoles.Admin))
            return;

        var dialog = new UserCardContentDialog(0)
        {
            XamlRoot = XamlRoot
        };

        await dialog.ShowAsync();

        if (ContentFrame.Content is IRefreshablePage page)
            await page.RefreshAsync();
    }
}
