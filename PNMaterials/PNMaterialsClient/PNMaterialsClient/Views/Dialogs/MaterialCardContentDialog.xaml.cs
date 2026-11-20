using PNMaterialsClient.ViewModels;
using PNMaterialsContracts;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class MaterialCardContentDialog : ContentDialog
{
    public ICommand ExitCommand { get; }
    public MaterialCardViewModel ViewModel { get; }
    public int? RequestedMaterialId { get; private set; }

    public MaterialCardContentDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MaterialCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
    }

    private void OnCreateRequestClick(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.IsAuthorized(PNRoles.ReadAndWrite))
            return;

        RequestedMaterialId = ViewModel.Id;
        Hide();
    }
}
