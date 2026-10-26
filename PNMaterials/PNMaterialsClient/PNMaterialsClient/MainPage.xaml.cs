using System.Collections.Specialized;
using Microsoft.UI;
using PNMaterialsClient.Navigation;
using PNMaterialsClient.Services;
using PNMaterialsClient.Views;
using PNMaterialsClient.Views.Dialogs;

namespace PNMaterialsClient;

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
    ];

    private readonly Dictionary<Type, MenuBarItem> _sectionByPage = [];

    private MessagesService MessagesService { get; }

    public MainPage()
    {
        this.InitializeComponent();
        MessagesService = App.Services.GetRequiredService<MessagesService>();
        BuildMenu();
        ContentFrame.Navigated += OnContentFrameNavigated;
        ContentFrame.Navigate(typeof(MaterialsSplitPage));
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        XamlRoot?.Changed += OnXamlRootChanged;
        MessagesService.Messages.CollectionChanged += RedrawPopup;
        ResizePopup();
        InfoPopup.IsOpen = true;
    }

    private void BuildMenu()
    {
        foreach (var folder in NavItems.OfType<NavFolder>())
        {
            var menuItem = new MenuBarItem
            {
                Title = folder.Title,
                BorderThickness = new Thickness(0, 0, 0, 2),
                BorderBrush = new SolidColorBrush(Colors.Transparent)
            };

            foreach (var page in folder.Children.OfType<NavPage>())
            {
                var flyoutItem = new MenuFlyoutItem
                {
                    Text = page.Title,
                    Tag = page
                };
                flyoutItem.Click += OnMenuItemClick;

                menuItem.Items.Add(flyoutItem);
                _sectionByPage[page.PageType] = menuItem;
            }

            MainMenu.Items.Add(menuItem);
        }
    }

    private void OnMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { Tag: NavPage page }
            && ContentFrame.CurrentSourcePageType != page.PageType)
        {
            ContentFrame.Navigate(page.PageType);
        }
    }

    private void OnBackClick(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.CanGoBack)
            ContentFrame.GoBack();
    }

    private void OnForwardClick(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.CanGoForward)
            ContentFrame.GoForward();
    }

    private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
    {
        BackButton.IsEnabled = ContentFrame.CanGoBack;
        ForwardButton.IsEnabled = ContentFrame.CanGoForward;

        if (_sectionByPage.TryGetValue(e.SourcePageType, out var section))
            HighlightSection(section);
    }

    private void HighlightSection(MenuBarItem active)
    {
        var accent = (Brush)Application.Current.Resources["AccentBrush"];

        foreach (var item in MainMenu.Items)
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
    }

    private void RedrawPopup(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InfoPopup.IsOpen = false;
        InfoPopup.IsOpen = true;
    }

    private void ResizePopup()
    {
        PopupGrid.Width = XamlRoot?.Size.Width ?? 0;
        PopupGrid.Height = XamlRoot?.Size.Height ?? 0;
    }

    private void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        ResizePopup();
    }
}
