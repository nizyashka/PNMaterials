using PNMaterialsClient.ViewModels;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class UserProfileContentDialog : ContentDialog
{
    private readonly ProfileCardViewModel ViewModel;
    private readonly int _id;

    public ICommand ExitCommand { get; }

    public UserProfileContentDialog(int id)
    {
        this.InitializeComponent();

        _id = id;
        ViewModel = App.Services.GetRequiredService<ProfileCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
        Opened += OnOpened;
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id);
    }
}
