using System.ComponentModel.DataAnnotations;

namespace PNMaterialsContracts;

public class PurchaseRequestEditDto
{
    public DateTime? DeliveryDate { get; set; }

    [MinLength(1, ErrorMessage = "Заявка должна содержать хотя бы одну позицию")]
    public List<PurchaseRequestItemEditDto> Items { get; set; } = new();
}