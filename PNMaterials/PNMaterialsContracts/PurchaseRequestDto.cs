namespace PNMaterialsContracts;

public class PurchaseRequestDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime DeliveryDate { get; set; }

    public RequestStatusDto Status { get; set; } = new();
    public List<RequestStatusDto> AllowedTransitions { get; set; } = new();

    public List<PurchaseRequestItemDto> Items { get; set; } = new();
}