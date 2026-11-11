namespace PNMaterialsClient.ViewModels;

public partial class RoleSelectionViewModel : ObservableObject
{
    public string Name { get; set; }

    [ObservableProperty]
    private bool _isSelected;
}
