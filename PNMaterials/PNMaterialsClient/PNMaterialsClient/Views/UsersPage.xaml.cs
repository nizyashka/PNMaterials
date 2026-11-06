using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views;

public sealed partial class UsersPage : Page
{
    public UsersViewModel ViewModel { get; }

    public UsersPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<UsersViewModel>();
    }
}
