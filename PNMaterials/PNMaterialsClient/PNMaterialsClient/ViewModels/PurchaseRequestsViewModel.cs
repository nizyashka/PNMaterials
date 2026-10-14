using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class PurchaseRequestsViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    public ObservableCollection<PurchaseRequestDto> Requests { get; } = new();

    public PurchaseRequestsViewModel(ApiClient api, ILogger<PurchaseRequestsViewModel> logger)
        : base(logger)
    {
        _api = api;
    }

    public async Task LoadAsync()
    {
        ShowInfo("Загрузка…");

        await RunAsync("Загрузка списка заявок", async () =>
        {
            var requests = await _api.GetPurchaseRequestsAsync();

            Requests.Clear();
            foreach (var r in requests)
                Requests.Add(r);

            ShowInfo($"Загружено заявок: {Requests.Count}");
        });
    }
}
