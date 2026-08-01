namespace PNMaterialsContracts;

public class MaterialDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;

    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
}