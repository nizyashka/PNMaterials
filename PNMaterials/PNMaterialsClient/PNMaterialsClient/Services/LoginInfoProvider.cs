namespace PNMaterialsClient.Services;

public sealed class LoginInfoProvider
{
    private string? _accessToken;
    private string? _username;
    private string? _password;

    public string? AccessToken => _accessToken;
    public string? Username => _username;
    public string? Password => _password;

    public void SetLoginInfo(string accessToken, string login, string password)
    {
        Interlocked.Exchange(ref _accessToken, accessToken);
        Interlocked.Exchange(ref _username, login);
        Interlocked.Exchange(ref _password, password);
    }

    public void ClearLoginInfo()
    {
        Interlocked.Exchange(ref _accessToken, null);
        Interlocked.Exchange(ref _username, null);
        Interlocked.Exchange(ref _password, null);
    }

    public void SetAccessToken(string? token)
    {
        Interlocked.Exchange(ref _accessToken, token);
    }

    public void ClearAccessToken()
    {
        Interlocked.Exchange(ref _accessToken, null);
    }
}
