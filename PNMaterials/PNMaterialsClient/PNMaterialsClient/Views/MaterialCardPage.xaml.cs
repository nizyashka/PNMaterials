using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views;

public sealed partial class MaterialCardPage : Page
{
    public MaterialCardViewModel ViewModel { get; }
    public ICommand ExitCommand { get; }

    public MaterialCardPage()
    {
        this.InitializeComponent();

        ViewModel = new MaterialCardViewModel(App.Api);
        ExitCommand = new RelayCommand(() =>
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        });
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var id = e.Parameter is int value ? value : 0;
        await ViewModel.LoadAsync(id);
    }
}
