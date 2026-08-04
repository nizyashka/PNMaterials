using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class RequestItemViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MaterialName))]
    [NotifyPropertyChangedFor(nameof(UnitName))]
    private MaterialDto? _material;

    [ObservableProperty]
    private double _quantity = 1;

    [ObservableProperty]
    private string _positionText = string.Empty;

    public string MaterialName => Material?.Name ?? string.Empty;
    public string UnitName => Material?.UnitName ?? string.Empty;

    public IRelayCommand? RemoveCommand { get; set; }
}
