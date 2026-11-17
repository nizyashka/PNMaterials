using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class UserCardContentDialog : ContentDialog
{
    public readonly UserCardViewModel ViewModel;

    public ICommand ExitCommand { get; }

    public UserCardContentDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<UserCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
    }
}
