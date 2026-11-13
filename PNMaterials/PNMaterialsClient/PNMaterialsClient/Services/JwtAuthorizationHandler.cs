using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using PNMaterialsContracts;

namespace PNMaterialsClient.Services;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly AuthService _authService;
    private readonly LoginInfoProvider _loginInfoProvider;

    public JwtAuthorizationHandler(LoginInfoProvider loginInfoProvider, AuthService authService)
    {
        _loginInfoProvider = loginInfoProvider;
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_loginInfoProvider.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _loginInfoProvider.AccessToken);

        var result = await base.SendAsync(request, cancellationToken);

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            try
            {
                await _authService.Relogin();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _loginInfoProvider.AccessToken);
                result = await base.SendAsync(request, cancellationToken);
            }
            catch (ApiException ex) when (ex.Kind is ApiErrorKind.Unauthorized)
            {
                throw new ApiException(ApiErrorKind.Unauthorized, "Повторите вход в приложение.", HttpStatusCode.Unauthorized, ex);
            }
        }

        return result;
    }
}
