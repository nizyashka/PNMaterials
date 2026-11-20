using System.Collections.Specialized;
using PNMaterialsClient.Services;

namespace PNMaterialsClient.Views;

public sealed partial class RootPage : Page
{
    private MessagesService MessagesService { get; }
    private LoadingService LoadingService { get; }

    public RootPage()
    {
        this.InitializeComponent();
        MessagesService = App.Services.GetRequiredService<MessagesService>();
        LoadingService = App.Services.GetRequiredService<LoadingService>();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        XamlRoot?.Changed += OnXamlRootChanged;
        MessagesService.Messages.CollectionChanged += RedrawPopup;
        
        ResizePopup();
        InfoPopup.IsOpen = true;

        if (RootPageFrame.Content is null)
            RootPageFrame.Navigate(typeof(LoginPage));
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        XamlRoot?.Changed -= OnXamlRootChanged;
        MessagesService.Messages.CollectionChanged -= RedrawPopup;
    }

    private void RedrawPopup(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            InfoPopup.IsOpen = false;
            InfoPopup.IsOpen = true;
        }
    }

    private void ResizePopup()
    {
        PopupGrid.Width = XamlRoot?.Size.Width ?? 0;
        PopupGrid.Height = XamlRoot?.Size.Height ?? 0;

        ProgressGrid.Width = XamlRoot?.Size.Width ?? 0;
        ProgressGrid.Height = XamlRoot?.Size.Height ?? 0;
    }

    private void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        ResizePopup();
    }
}
