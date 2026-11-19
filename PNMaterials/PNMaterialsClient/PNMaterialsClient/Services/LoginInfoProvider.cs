using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Office;

namespace PNMaterialsClient.Services;

public sealed class LoginInfoProvider
{
    public int? Id { get; private set; }
    public string? AccessToken { get; private set; }
    public string? Username { get; private set; }
    public string? Password { get; private set; }
    public List<string> Roles { get; private set; } = new List<string>();

    public void SetLoginInfo(int id, string accessToken, string username, string password, List<string> roles)
    {
        Id = id;
        AccessToken = accessToken;
        Username = username;
        Password = password;
        Roles = roles;
    }

    public void ClearAll()
    {
        Id = null;
        AccessToken = null;
        Username = null;
        Password = null;
        Roles = new List<string>();
    }

    public void SetAccessToken(string accessToken)
    {
        AccessToken = accessToken;
    }

    public void SetUsername(string username)
    {
        Username = username;
    }

    public void SetPassword(string password)
    {
        Password = password;
    }

    public void SetRoles(List<string> roles)
    {
        Roles = roles;
    }
}
