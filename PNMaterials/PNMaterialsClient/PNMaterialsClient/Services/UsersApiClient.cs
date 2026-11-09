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

    public async Task<List<UserDto>> GetAllUsers()
    {
        var response = await ApiRequestExecutor.SendAsync(_http.GetAsync("api/users"));
        return await ApiRequestExecutor.ReadAsync<List<UserDto>>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул пользователей.");
    }

    public async Task<UserDto> CreateUser(CreateUserDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync("api/users", dto));
        return await ApiRequestExecutor.ReadAsync<UserDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул созданного пользователя.");
    }

    public async Task<ProfileDto> EditProfile(int id, ProfileEditDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PutAsJsonAsync($"api/users/{id}/profile", dto));
        return await ApiRequestExecutor.ReadAsync<ProfileDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул измененный профиль пользователя.");
    }

    public async Task EditPassword(int id, PasswordEditDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PutAsJsonAsync($"api/users/{id}/password", dto));
    }

    public async Task<RolesDto> EditRoles(int id, RolesDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PutAsJsonAsync($"api/users/{id}/roles", dto));
        return await ApiRequestExecutor.ReadAsync<RolesDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул список измененных ролей пользователя.");
    }
}
