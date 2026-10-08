using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class MaterialCardContentDialog : ContentDialog
{
    private readonly int _id;
    public ICommand ExitCommand { get; }

    public MaterialCardViewModel ViewModel { get; }

    public MaterialCardContentDialog(int id)
    {
        this.InitializeComponent();
        _id = id;
        ViewModel = new MaterialCardViewModel(App.Api);
        ExitCommand = new RelayCommand(Hide);
        Opened += OnOpened;
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id);
    }

    private void OnCloseClick(object sender, ItemClickEventArgs e)
    {
        Hide();
    }
}
