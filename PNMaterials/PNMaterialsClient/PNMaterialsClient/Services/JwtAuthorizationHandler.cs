using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using PNMaterialsContracts;

namespace PNMaterialsClient.Services;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private LoginInfoProvider _loginProvider { get; }
    private AuthApiClient _authApiClient { get; }

    public JwtAuthorizationHandler(LoginInfoProvider loginProvider, AuthApiClient authApiClient)
    {
        _loginProvider = loginProvider;
        _authApiClient = authApiClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_loginProvider.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _loginProvider.AccessToken);

        var result = await base.SendAsync(request, cancellationToken);

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (string.IsNullOrWhiteSpace(_loginProvider.Username) || string.IsNullOrWhiteSpace(_loginProvider.Password))
                throw new ApiException(ApiErrorKind.Unauthorized, "Не выполнен вход в приложение.");

            try
            {
                var loginResult = await _authApiClient.Login(new LoginDto { Username = _loginProvider.Username, Password = _loginProvider.Password });
                _loginProvider.SetAccessToken(loginResult.AccessToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _loginProvider.AccessToken);
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
