using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using PNMaterialsContracts;

namespace PNMaterialsClient.Services;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private AccessTokenProvider _tokenProvider { get; }
    private AuthApiClient _authApiClient { get; }

    public JwtAuthorizationHandler(AccessTokenProvider tokenProvider, AuthApiClient authApiClient)
    {
        _tokenProvider = tokenProvider;
        _authApiClient = authApiClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_tokenProvider.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);

        var result = await base.SendAsync(request, cancellationToken);

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (string.IsNullOrWhiteSpace(_tokenProvider.RefreshToken))
                return result;

            var tokenResponse = await _authApiClient.Refresh(new RefreshTokenRequest { RefreshToken = _tokenProvider.RefreshToken });
            _tokenProvider.SetAccessToken(tokenResponse.AccessToken);
            _tokenProvider.SetRefreshToken(tokenResponse.RefreshToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
            result = await base.SendAsync(request, cancellationToken);
        }

        return result;
    }
}
