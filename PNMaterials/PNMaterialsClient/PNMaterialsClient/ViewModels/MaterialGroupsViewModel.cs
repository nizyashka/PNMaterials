using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialGroupsViewModel : ObservableObject
{
    private readonly ApiClient _api;
    private readonly ILogger<MaterialGroupsViewModel> _logger;

    [ObservableProperty]
    private string? _status;

    public ObservableCollection<MaterialGroupDto> Groups { get; } = new();

    public MaterialGroupsViewModel(ApiClient api, ILogger<MaterialGroupsViewModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        Status = "Загрузка…";
        try
        {
            var groups = await _api.GetMaterialGroupsAsync();

            Groups.Clear();
            foreach (var g in groups)
                Groups.Add(g);

            Status = $"Загружено групп: {Groups.Count}";
        }
        catch (Exception ex)
        {
            Status = $"Ошибка: {ex.Message}";
        }
    }
}
