using System.Text.Json.Serialization;

namespace PNMaterialsContracts;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string[] Roles { get; set; }

    [JsonIgnore]
    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

    [JsonIgnore]
    public string RolesString => string.Join(", ", Roles);
}
