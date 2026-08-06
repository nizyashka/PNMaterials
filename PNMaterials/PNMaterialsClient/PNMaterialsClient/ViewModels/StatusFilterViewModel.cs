using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class StatusFilterViewModel : ObservableObject
{
    public StatusFilterViewModel(RequestStatusDto status)
    {
        Status = status;
    }

    public RequestStatusDto Status { get; }

    public string Title => $"{Status.Code} — {Status.Name}";

    [ObservableProperty]
    private bool _isSelected;
}
