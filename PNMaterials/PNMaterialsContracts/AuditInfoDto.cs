namespace PNMaterialsContracts;

public class AuditInfoDto
{
    public string CreatedByDisplayName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public string? EditedByDisplayName { get; set; }
    public DateTime? EditedAt { get; set; }
}
