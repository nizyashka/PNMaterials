using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialGroupsViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    public ObservableCollection<MaterialGroupDto> Groups { get; } = new();

    public MaterialGroupsViewModel(ApiClient api, ILogger<MaterialGroupsViewModel> logger)
        : base(logger)
    {
        _api = api;
    }

    public async Task LoadAsync()
    {
        MessagesService.ShowInfo("Загрузка…");

        await RunAsync("Загрузка списка групп", async () =>
        {
            var groups = await _api.GetMaterialGroupsAsync();

            Groups.Clear();
            foreach (var g in groups)
                Groups.Add(g);

            MessagesService.ShowInfo($"Загружено групп: {Groups.Count}");
        });
    }
}
