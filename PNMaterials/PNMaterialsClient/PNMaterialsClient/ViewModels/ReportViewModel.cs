using System.Collections.ObjectModel;
using PNMaterialsClient.Services;
using PNMaterialsContracts;

namespace PNMaterialsClient.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    private readonly ApiClient _api;

    public ReportViewModel(ApiClient api)
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

    [ObservableProperty]
    private string? _status;

    public ObservableCollection<StatusFilterViewModel> Statuses { get; } = new();
    public ObservableCollection<PurchaseRequestReportRowDto> Rows { get; } = new();

    public async Task LoadAsync()
    {
        if (Statuses.Count > 0)
            return;

        try
        {
            foreach (var status in await _api.GetRequestStatusesAsync())
                Statuses.Add(new StatusFilterViewModel(status));
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        Status = "Поиск…";
        IsEmptyResult = false;

        try
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
            Status = Rows.Count == 0 ? null : $"Найдено заявок: {Rows.Count}";
        }
        catch (Exception ex)
        {
            IsEmptyResult = false;
            Status = ex.Message;
        }
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
        Status = null;
    }

    private static List<string> ParseValues(string text)
        => text.Split(
                new[] { ',', ';', ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .ToList();
}
