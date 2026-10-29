using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace PNMaterialsClient.Services;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private AccessTokenProvider _tokenProvider { get; }

    public JwtAuthorizationHandler(AccessTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenProvider.AccessToken;

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}
