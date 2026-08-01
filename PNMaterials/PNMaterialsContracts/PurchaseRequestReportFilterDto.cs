namespace PNMaterialsContracts;

public class PurchaseRequestReportFilterDto
{
    public List<string> RequestNumbers { get; set; } = new();
    public List<string> MaterialCodes { get; set; } = new();
    public string? MaterialName { get; set; }

    public DateTime? DeliveryDateFrom { get; set; }
    public DateTime? DeliveryDateTo { get; set; }

    public List<int> StatusIds { get; set; } = new();
    public bool StatusNotEqual { get; set; }
}