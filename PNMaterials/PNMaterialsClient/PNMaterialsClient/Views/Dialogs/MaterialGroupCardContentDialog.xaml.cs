using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class MaterialGroupCardContentDialog : ContentDialog
{
    private readonly int _id;

    public MaterialGroupCardViewModel ViewModel { get; }
    public ICommand ExitCommand { get; }

    public MaterialGroupCardContentDialog(int id)
    {
        this.InitializeComponent();
        _id = id;
        ViewModel = App.Services.GetRequiredService<MaterialGroupCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
        Opened += OnOpened;
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id);
    }
}
