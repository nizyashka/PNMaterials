using Microsoft.AspNetCore.Identity;
using PNMaterialsDomain.Entities;

namespace PNMaterialsInfrastructure.Identity;

public class PNUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public bool IsBlocked { get; set; } = false;

    public AuditInfo Audit { get; set; } = null!;
}
