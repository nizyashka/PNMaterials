using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class PurchaseRequestsViewModel : ObservableObject
{
    private readonly ApiClient _api;

    [ObservableProperty]
    private string? _status;

    public ObservableCollection<PurchaseRequestDto> Requests { get; } = new();

    public PurchaseRequestsViewModel(ApiClient api)
    {
        _api = api;
    }

    public async Task LoadAsync()
    {
        Status = "Загрузка…";
        try
        {
            var requests = await _api.GetPurchaseRequestsAsync();

            Requests.Clear();
            foreach (var r in requests)
                Requests.Add(r);

            Status = $"Загружено заявок: {Requests.Count}";
        }
        catch (Exception ex)
        {
            Status = $"Ошибка: {ex.Message}";
        }
    }
}
