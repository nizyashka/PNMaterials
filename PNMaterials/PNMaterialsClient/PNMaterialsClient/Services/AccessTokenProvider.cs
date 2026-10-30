namespace PNMaterialsClient.Services;

public sealed class AccessTokenProvider
{
    private string? _accessToken;
    private string? _refreshToken;

    public string? AccessToken => _accessToken;
    public string? RefreshToken => _refreshToken;

    public void SetAccessToken(string? token)
    {
        Interlocked.Exchange(ref _accessToken, token);
    }

    public void SetRefreshToken(string? token)
    {
        Interlocked.Exchange(ref _refreshToken, token);
    }

    public void ClearAccessToken()
    {
        Interlocked.Exchange(ref _accessToken, null);
    }

    public void ClearRefreshToken()
    {
        Interlocked.Exchange(ref _refreshToken, null);
    }
}
