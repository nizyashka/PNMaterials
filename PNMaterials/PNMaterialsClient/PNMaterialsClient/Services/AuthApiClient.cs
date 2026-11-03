using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using PNMaterialsContracts;

namespace PNMaterialsClient.Services;

public class AuthApiClient
{
    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<LoginResultDto> Login(LoginDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync("api/auth/login", dto));
        return await ApiRequestExecutor.ReadAsync<LoginResultDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул результат логина.");
    }

    public async Task<TokenResponse> Refresh(RefreshTokenRequest dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync("api/auth/refresh", dto));
        return await ApiRequestExecutor.ReadAsync<TokenResponse>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул результат обновления токена.");
    }
}
