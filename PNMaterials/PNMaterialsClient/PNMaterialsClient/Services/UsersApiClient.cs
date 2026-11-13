using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using PNMaterialsContracts;

namespace PNMaterialsClient.Services;

public class UsersApiClient
{
    private readonly HttpClient _http;

    public UsersApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var response = await ApiRequestExecutor.SendAsync(_http.GetAsync("api/users"));
        return await ApiRequestExecutor.ReadAsync<List<UserDto>>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул пользователей.");
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.GetAsync($"api/users/{id}"));
        return await ApiRequestExecutor.ReadAsync<UserDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул пользователя.");
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync("api/users", dto));
        return await ApiRequestExecutor.ReadAsync<UserDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул созданного пользователя.");
    }

    public async Task<UserDto> EditUserAsync(int id, UserEditDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PutAsJsonAsync($"api/users/{id}", dto));
        return await ApiRequestExecutor.ReadAsync<UserDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул измененный профиль пользователя.");
    }

    public async Task<List<string>> GetAllRoles()
    {
        var response = await ApiRequestExecutor.SendAsync(_http.GetAsync($"api/users/roles"));
        return await ApiRequestExecutor.ReadAsync<List<string>>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул список ролей.");
    }

    public async Task BlockUserAsync(int id)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PutAsync($"api/users/{id}/block", null));
    }

    public async Task UnblockUserAsync(int id)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PutAsync($"api/users/{id}/unblock", null));
    }
}
