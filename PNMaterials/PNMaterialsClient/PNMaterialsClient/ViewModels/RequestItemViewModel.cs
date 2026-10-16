using PNMaterialsContracts;
using System.Collections.ObjectModel;

namespace PNMaterialsClient.ViewModels;

public partial class RequestItemViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MaterialName))]
    [NotifyPropertyChangedFor(nameof(UnitName))]
    [NotifyPropertyChangedFor(nameof(UnitAllowsFractions))]
    private MaterialDto? _material;

    [ObservableProperty]
    private double _quantity = 1;

    [ObservableProperty]
    private string _positionText = string.Empty;

    [ObservableProperty]
    private bool _isEditMode;

    public string MaterialName => Material?.Name ?? string.Empty;
    public string UnitName => Material?.UnitName ?? string.Empty;
    public bool UnitAllowsFractions => Material?.UnitAllowsFractions ?? true;

    public IRelayCommand? RemoveCommand { get; set; }

    public ObservableCollection<MaterialDto> Materials { get; init; } = new();
}
