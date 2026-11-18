using System.ComponentModel.DataAnnotations.Schema;

namespace PNMaterialsDomain.Entities;

[ComplexType]
public record AuditInfo
{
    public int? CreatedByUserId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public int? EditedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
}
