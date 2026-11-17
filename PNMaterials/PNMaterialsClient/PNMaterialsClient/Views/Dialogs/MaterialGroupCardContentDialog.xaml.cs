using System.ComponentModel;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class MaterialGroupCardContentDialog : ContentDialog
{
    public MaterialGroupCardViewModel ViewModel { get; }
    public ICommand ExitCommand { get; }

    public MaterialGroupCardContentDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MaterialGroupCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
    }
}
