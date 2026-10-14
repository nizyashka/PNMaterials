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
    private bool _statusNotEqual;

    [ObservableProperty]
    private bool _isEmptyResult;

    public ObservableCollection<StatusFilterViewModel> Statuses { get; } = new();
    public ObservableCollection<PurchaseRequestReportRowDto> Rows { get; } = new();

    public async Task LoadAsync()
    {
        if (Statuses.Count > 0)
            return;

        await RunAsync("Загрузка статусов", async () =>
        {
            foreach (var status in await _api.GetRequestStatusesAsync())
                Statuses.Add(new StatusFilterViewModel(status));
        });
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        ShowInfo("Поиск…");
        IsEmptyResult = false;

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

            IsEmptyResult = Rows.Count == 0;

            if (Rows.Count == 0)
                ClearMessage();
            else
                ShowInfo($"Найдено заявок: {Rows.Count}");
        });
    }

    [RelayCommand]
    private void Clear()
    {
        RequestNumbersText = string.Empty;
        MaterialCodesText = string.Empty;
        MaterialName = string.Empty;
        DeliveryFrom = null;
        DeliveryTo = null;
        StatusNotEqual = false;

        foreach (var status in Statuses)
            status.IsSelected = false;

        Rows.Clear();
        IsEmptyResult = false;
        ClearMessage();
    }

    private static List<string> ParseValues(string text)
        => text.Split(
                new[] { ',', ';', ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .ToList();
}
