using System.Text.Json.Serialization;

namespace PNMaterialsContracts;

public class RequestStatusDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public string Title => $"{Code} — {Name}";
}