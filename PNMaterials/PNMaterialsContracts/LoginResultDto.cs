namespace PNMaterialsContracts;

public class LoginResultDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset Expiration { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string[] Roles { get; set; } = [];
}
