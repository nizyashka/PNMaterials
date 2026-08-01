namespace PNMaterialsContracts;

public class PurchaseRequestReportRowDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public RequestStatusDto Status { get; set; } = new();
}