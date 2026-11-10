namespace PNMaterialsClient.Services;

public sealed class LoginInfoProvider
{
    private int? _id;
    private string? _accessToken;
    private string? _username;
    private string? _password;

    public int? Id => _id;
    public string? AccessToken => _accessToken;
    public string? Username => _username;
    public string? Password => _password;

    public void SetLoginInfo(int id, string accessToken, string login, string password)
    {
        _id = id;
        Interlocked.Exchange(ref _accessToken, accessToken);
        Interlocked.Exchange(ref _username, login);
        Interlocked.Exchange(ref _password, password);
    }

    public void ClearLoginInfo()
    {
        _id = null;
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

    public void SetUsername(string? username)
    {
        Interlocked.Exchange(ref _username, username);
    }

    public void SetPassword(string? password)
    {
        Interlocked.Exchange(ref _password, password);
    }
}
