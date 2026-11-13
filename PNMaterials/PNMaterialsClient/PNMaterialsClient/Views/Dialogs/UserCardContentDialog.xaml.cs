using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class UserCardContentDialog : ContentDialog
{
    private readonly UserCardViewModel ViewModel;
    private readonly int _id;

    public ICommand ExitCommand { get; }

    public UserCardContentDialog(int id)
    {
        this.InitializeComponent();
        _id = id;
        ViewModel = App.Services.GetRequiredService<UserCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
        Opened += OnOpened;
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id);
    }
}
