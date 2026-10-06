using PNMaterialsClient.Navigation;
using PNMaterialsClient.Views;

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
                new NavPage { Title = "Материалы", PageType = typeof(MaterialsPage) },
                new NavPage { Title = "Группы материалов", PageType = typeof(MaterialGroupsPage) },
            ]
        },
        new NavFolder
        {
            Title = "Заявки",
            Children =
            [
                new NavPage { Title = "Список заявок", PageType = typeof(PurchaseRequestsPage) },
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

    public MainPage()
    {
        this.InitializeComponent();
        ContentFrame.Navigate(typeof(MaterialsPage));
    }

    private void OnNavItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        switch (args.InvokedItem)
        {
            case NavPage page when ContentFrame.CurrentSourcePageType != page.PageType:
                ContentFrame.Navigate(page.PageType);
                break;

            case NavFolder folder:
                if (sender.ContainerFromItem(folder) is TreeViewItem container)
                    container.IsExpanded = !container.IsExpanded;
                break;
        }
    }
}
