namespace PNMaterialsDomain.Entities;

public class MaterialGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public AuditInfo Audit { get; set; } = null!;
}
