namespace PNMaterialsClient.Services;

public sealed class LoginInfoProvider
{
    public int? Id { get; private set; }
    public string? AccessToken { get; private set; }
    public string? Username { get; private set; }
    public string? Password { get; private set; }
    public List<string>? Roles { get; private set; }

    public void SetLoginInfo(int id, string accessToken, string username, string password, List<string> roles)
    {
        Id = id;
        AccessToken = accessToken;
        Username = username;
        Password = password;
        Roles = roles;
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
