using System.Collections.ObjectModel;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using PNMaterialsClient.Services;
using PNMaterialsContracts;
using Windows.Storage.Pickers;

namespace PNMaterialsClient.ViewModels;

public partial class ReportViewModel : ViewModelBase
{
    private readonly ApiClient _api;

    public ReportViewModel(ApiClient api, ILogger<ReportViewModel> logger, AuthService authService) : base(logger, authService)
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

    [ObservableProperty]
    private bool _hasReportResult;

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
        });
    }

    [RelayCommand]
    public async Task SearchAsync()
    {
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

            HasReportResult = true;

            Rows.Clear();
            foreach (var row in rows)
                Rows.Add(row);

            if (Rows.Count == 0)
                MessagesService.ShowInfo("Заявок по заданным критериям не найдено.");
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

        RefreshStatusesSummary();
    }

    [RelayCommand]
    private async Task ExportReportAsync()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Заявки"
        };

        picker.FileTypeChoices.Add("Таблица Excel", new List<string> { ".xlsx" });

        var file = await picker.PickSaveFileAsync();

        if (file is not null)
        {
            var path = file.Path;

            await RunAsync("Загрузка заявок для выгрузки.", async () =>
            {
                var requests = await _api.GetPurchaseRequestsListAsync(Rows.Select(r => r.Id));

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Заявки");

                worksheet.Cell("A1").Value = "Код";
                worksheet.Cell("B1").Value = "Дата создания";
                worksheet.Cell("C1").Value = "Дата поставки";
                worksheet.Cell("D1").Value = "Статус";
                worksheet.Cell("E1").Value = "Код материала";
                worksheet.Cell("F1").Value = "Наименование материала";
                worksheet.Cell("G1").Value = "Единица измерения";
                worksheet.Cell("H1").Value = "Количество";
                worksheet.Cell("I1").Value = "Описание";

                var data = requests.SelectMany(row
                    => row.Items.Select(i => new object[]
                {
                    row.Number,
                    row.CreatedAt,
                    row.DeliveryDate,
                    row.Status.Title,
                    i.MaterialCode,
                    i.MaterialName,
                    i.UnitName,
                    i.Quantity,
                    i.PositionText ?? "Описание отсутствует."
                }));

                worksheet.Cell("A2").InsertData(data);
                worksheet.Column("B").Style.NumberFormat.Format = "dd.mm.yyyy hh:mm";
                worksheet.Column("C").Style.NumberFormat.Format = "dd.mm.yyyy";
                worksheet.ColumnsUsed().AdjustToContents();

                workbook.SaveAs(path);
            });
        }
    }

    private static List<string> ParseValues(string text)
        => text.Split(
                new[] { ',', ';', ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .ToList();
}
