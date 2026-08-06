using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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

    public async Task<MaterialGroupDto?> GetMaterialGroupAsync(int id)
    {
        var response = await _http.GetAsync($"api/materialgroups/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<MaterialGroupDto>();
    }

    public async Task<MaterialGroupDto> CreateMaterialGroupAsync(MaterialGroupEditDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/materialgroups", dto);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<MaterialGroupDto>()
               ?? throw new ApiException("Сервер не вернул созданную группу.");
    }

    public async Task UpdateMaterialGroupAsync(int id, MaterialGroupEditDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/materialgroups/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync();
        throw new ApiException(ExtractMessage(body, response.StatusCode));
    }

    private static string ExtractMessage(string body, HttpStatusCode status)
    {
        if (string.IsNullOrWhiteSpace(body))
            return $"Ошибка сервера ({(int)status}).";

        try
        {
            using var doc = JsonDocument.Parse(body);

            if (doc.RootElement.TryGetProperty("errors", out var errors))
            {
                var messages = errors.EnumerateObject()
                    .SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString()))
                    .Where(m => !string.IsNullOrWhiteSpace(m));

                return string.Join(Environment.NewLine, messages);
            }

            if (doc.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? body;
        }
        catch (JsonException)
        {
        }

        return body;
    }

    public async Task<List<UnitDto>> GetUnitsAsync()
    => await _http.GetFromJsonAsync<List<UnitDto>>("api/units") ?? new();

    public async Task<List<MaterialDto>> GetMaterialsAsync()
        => await _http.GetFromJsonAsync<List<MaterialDto>>("api/materials") ?? new();

    public async Task<MaterialDto?> GetMaterialAsync(int id)
    {
        var response = await _http.GetAsync($"api/materials/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<MaterialDto>();
    }

    public async Task<MaterialDto> CreateMaterialAsync(MaterialEditDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/materials", dto);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<MaterialDto>()
               ?? throw new ApiException("Сервер не вернул созданный материал.");
    }

    public async Task UpdateMaterialAsync(int id, MaterialEditDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/materials/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    public async Task<List<PurchaseRequestDto>> GetPurchaseRequestsAsync()
    => await _http.GetFromJsonAsync<List<PurchaseRequestDto>>("api/purchaserequests") ?? new();

    public async Task<PurchaseRequestDto?> GetPurchaseRequestAsync(int id)
    {
        var response = await _http.GetAsync($"api/purchaserequests/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<PurchaseRequestDto>();
    }

    public async Task<PurchaseRequestDto> CreatePurchaseRequestAsync(PurchaseRequestEditDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/purchaserequests", dto);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<PurchaseRequestDto>()
               ?? throw new ApiException("Сервер не вернул созданную заявку.");
    }

    public async Task UpdatePurchaseRequestAsync(int id, PurchaseRequestEditDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/purchaserequests/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    public async Task<PurchaseRequestDto> ChangeRequestStatusAsync(int id, int statusId)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/purchaserequests/{id}/status",
            new RequestStatusChangeDto { StatusId = statusId });

        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<PurchaseRequestDto>()
               ?? throw new ApiException("Сервер не вернул обновлённую заявку.");
    }

    public async Task<List<RequestStatusDto>> GetRequestStatusesAsync()
    => await _http.GetFromJsonAsync<List<RequestStatusDto>>("api/purchaserequests/statuses") ?? new();

    public async Task<List<PurchaseRequestReportRowDto>> GetPurchaseRequestReportAsync(
        PurchaseRequestReportFilterDto filter)
    {
        var response = await _http.PostAsJsonAsync("api/reports/purchase-requests", filter);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<List<PurchaseRequestReportRowDto>>() ?? new();
    }
}
