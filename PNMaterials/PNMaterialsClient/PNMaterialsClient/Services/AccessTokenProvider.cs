using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsClient.Services;

public sealed class AccessTokenProvider
{
    private string? _accessToken;

    public string? AccessToken =>
        Volatile.Read(ref _accessToken);

    public void SetAccessToken(string? token)
    {
        Interlocked.Exchange(ref _accessToken, token);
    }

    public void Clear()
    {
        Interlocked.Exchange(ref _accessToken, null);
    }
}
