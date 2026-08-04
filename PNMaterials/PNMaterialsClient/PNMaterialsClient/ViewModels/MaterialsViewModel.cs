using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialsViewModel : ObservableObject
{
    private readonly ApiClient _api;

    [ObservableProperty]
    private string? _status;

    public ObservableCollection<MaterialDto> Materials { get; } = new();

    public MaterialsViewModel(ApiClient api)
    {
        _api = api;
    }

    public async Task LoadAsync()
    {
        Status = "Загрузка…";
        try
        {
            var materials = await _api.GetMaterialsAsync();

            Materials.Clear();
            foreach (var m in materials)
                Materials.Add(m);

            Status = $"Загружено материалов: {Materials.Count}";
        }
        catch (Exception ex)
        {
            Status = $"Ошибка: {ex.Message}";
        }
    }
}
