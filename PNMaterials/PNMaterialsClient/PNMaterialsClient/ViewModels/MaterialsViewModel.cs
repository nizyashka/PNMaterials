using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class MaterialsViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGroupSelected))]
    private int? _groupId;

    public bool IsGroupSelected => GroupId.HasValue;

    public ObservableCollection<MaterialDto> Materials { get; } = new();

    public MaterialsViewModel(ApiClient api, ILogger<MaterialsViewModel> logger)
        : base(logger)
    {
        _api = api;
    }

    public async Task LoadAsync()
    {
        if (GroupId is not { } requestedGroupId)
        {
            ShowInfo("Выберите группу");
            return;
        }

        ShowInfo("Загрузка…");

        await RunAsync("Загрузка списка материалов", async () =>
        {
            var materials = await _api.GetMaterialsAsync(requestedGroupId);

            if (GroupId != requestedGroupId)
                return;

            Materials.Clear();
            foreach (var m in materials)
                Materials.Add(m);

            ShowInfo($"Загружено материалов: {Materials.Count}");
        });
    }

    public void ShowSelectGroupHint()
    {
        ShowInfo("Выберите группу");
    }
}
