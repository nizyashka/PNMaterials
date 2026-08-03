using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PNMaterialsContracts;

namespace PNMaterialsClient.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(string baseAddress)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseAddress) };
    }

    public async Task<List<MaterialGroupDto>> GetMaterialGroupsAsync()
        => await _http.GetFromJsonAsync<List<MaterialGroupDto>>("api/materialgroups") ?? new();
}