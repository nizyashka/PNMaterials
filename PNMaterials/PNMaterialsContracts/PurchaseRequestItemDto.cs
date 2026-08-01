namespace PNMaterialsContracts;

public class PurchaseRequestItemDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? PositionText { get; set; }
}