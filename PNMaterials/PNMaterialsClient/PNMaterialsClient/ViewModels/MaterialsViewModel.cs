using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialsViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    public ObservableCollection<MaterialDto> Materials { get; } = new();

    public MaterialsViewModel(ApiClient api, ILogger<MaterialsViewModel> logger)
        : base(logger)
    {
        _api = api;
    }

    public async Task LoadAsync()
    {
        ShowInfo("Загрузка…");

        await RunAsync("Загрузка списка материалов", async () =>
        {
            var materials = await _api.GetMaterialsAsync();

            Materials.Clear();
            foreach (var m in materials)
                Materials.Add(m);

            ShowInfo($"Загружено материалов: {Materials.Count}");
        });
    }
}
