using System;
using System.Collections.Generic;
using System.Text;
using PNMaterialsContracts;

namespace PNMaterialsClient.Services;

public class AuthService
{
    private readonly AuthApiClient _authApiClient;
    private readonly LoginInfoProvider _loginInfoProvider;

    public AuthService(AuthApiClient authApiClient, LoginInfoProvider loginInfoProvider)
    {
        _authApiClient = authApiClient;
        _loginInfoProvider = loginInfoProvider;
    }

    public async Task Login(string username, string password)
    {
        LoginDto loginDto = new LoginDto { Username = username, Password = password };
        LoginResultDto result = await _authApiClient.Login(loginDto);

        _loginInfoProvider.SetLoginInfo(result.Id, result.AccessToken, result.Username, password, result.Roles);
    }

    public async Task Relogin()
    {
        if (string.IsNullOrWhiteSpace(_loginInfoProvider.Username) || string.IsNullOrWhiteSpace(_loginInfoProvider.Password))
            throw new ApiException(ApiErrorKind.Unauthorized, "Не выполнен вход в приложение.");

        await Login(_loginInfoProvider.Username, _loginInfoProvider.Password);
    }
}
