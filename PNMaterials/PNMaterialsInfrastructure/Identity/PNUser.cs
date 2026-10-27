using Microsoft.AspNetCore.Identity;

namespace PNMaterialsInfrastructure.Identity;

public class PNUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
}
