using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class ReportViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    public ReportViewModel(ApiClient api, ILogger<ReportViewModel> logger)
        : base(logger)
    {
        _api = api;
    }

    [ObservableProperty]
    private string _requestNumbersText = string.Empty;

    [ObservableProperty]
    private string _materialCodesText = string.Empty;

    [ObservableProperty]
    private string _materialName = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _deliveryFrom;

    [ObservableProperty]
    private DateTimeOffset? _deliveryTo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusesSummary))]
    private bool _statusNotEqual;

    public ObservableCollection<StatusFilterViewModel> Statuses { get; } = new();
    public ObservableCollection<PurchaseRequestReportRowDto> Rows { get; } = new();

    public string StatusesSummary
    {
        get
        {
            var selected = Statuses.Where(s => s.IsSelected).Select(s => s.Status.Code).ToList();

            if (selected.Count == 0)
                return "Все статусы";

            var names = selected.Count <= 2 ? string.Join(", ", selected) : $"Выбрано: {selected.Count}";
            return StatusNotEqual ? $"Кроме: {names}" : names;
        }
    }

    public void RefreshStatusesSummary() => OnPropertyChanged(nameof(StatusesSummary));

    public async Task LoadAsync()
    {
        if (Statuses.Count > 0)
            return;

        await RunAsync("Загрузка статусов", async () =>
        {
            foreach (var status in await _api.GetRequestStatusesAsync())
                Statuses.Add(new StatusFilterViewModel(status));
            
            ShowInfo("Нажмите Поиск");
        });
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        ShowInfo("Поиск…");

        await RunAsync("Построение отчёта", async () =>
        {
            var filter = new PurchaseRequestReportFilterDto
            {
                RequestNumbers = ParseValues(RequestNumbersText),
                MaterialCodes = ParseValues(MaterialCodesText),
                MaterialName = string.IsNullOrWhiteSpace(MaterialName) ? null : MaterialName,
                DeliveryDateFrom = DeliveryFrom?.Date,
                DeliveryDateTo = DeliveryTo?.Date,
                StatusIds = Statuses.Where(s => s.IsSelected).Select(s => s.Status.Id).ToList(),
                StatusNotEqual = StatusNotEqual
            };

            var rows = await _api.GetPurchaseRequestReportAsync(filter);

            Rows.Clear();
            foreach (var row in rows)
                Rows.Add(row);

            if (Rows.Count == 0)
                ShowInfo("Заявок по заданным критериям не найдено.");
            else
                ShowInfo($"Найдено заявок: {Rows.Count}");
        });
    }

    [RelayCommand]
    private async Task ClearAsync()
    {
        RequestNumbersText = string.Empty;
        MaterialCodesText = string.Empty;
        MaterialName = string.Empty;
        DeliveryFrom = null;
        DeliveryTo = null;
        StatusNotEqual = false;

        foreach (var status in Statuses)
            status.IsSelected = false;

        RefreshStatusesSummary();
    }

    private static List<string> ParseValues(string text)
        => text.Split(
                new[] { ',', ';', ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .ToList();
}
