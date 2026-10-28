namespace PNMaterialsContracts;

public class LoginResultDto
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset Expiration { get; set; }
    public string Login { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string[] Roles { get; set; } = [];
}
