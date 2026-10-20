using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PNMaterialsContracts;

namespace PNMaterialsClient.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var kind = GetErrorKind(response.StatusCode);

        var message = kind switch
        {
            ApiErrorKind.Validation or ApiErrorKind.Conflict =>
                ExtractMessage(await response.Content.ReadAsStringAsync(), response.StatusCode),
            ApiErrorKind.NotFound => "Запись не найдена. Возможно, она была удалена.",
            ApiErrorKind.Server => "Ошибка на сервере. Попробуйте повторить позже.",
            _ => $"Неожиданный ответ сервера ({(int)response.StatusCode})."
        };

        throw new ApiException(kind, message, response.StatusCode);
    }

    private static ApiErrorKind GetErrorKind(HttpStatusCode status) => status switch
    {
        HttpStatusCode.BadRequest => ApiErrorKind.Validation,
        HttpStatusCode.NotFound => ApiErrorKind.NotFound,
        HttpStatusCode.Conflict => ApiErrorKind.Conflict,
        _ when (int)status >= 500 => ApiErrorKind.Server,
        _ => ApiErrorKind.Unexpected
    };

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

            if (doc.RootElement.TryGetProperty("detail", out var detail) && detail.GetString() is { Length: > 0 } detailText)
                return detailText;

            if (doc.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? body;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException) { }

        return body;
    }

    private static async Task<HttpResponseMessage> SendAsync(Task<HttpResponseMessage> request)
    {
        HttpResponseMessage response;

        try
        {
            response = await request;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(ApiErrorKind.Network,
                "Сервер недоступен. Проверьте подключение и повторите попытку.", innerException: ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ApiException(ApiErrorKind.Timeout,
                "Сервер не ответил вовремя. Повторите попытку позже.", innerException: ex);
        }

        await EnsureSuccessAsync(response);
        return response;
    }

    private static async Task<T?> ReadAsync<T>(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (JsonException ex)
        {
            throw new ApiException(ApiErrorKind.InvalidResponse,
                "Сервер вернул данные в неожиданном формате.", response.StatusCode, ex);
        }
    }

    public async Task<List<MaterialGroupDto>> GetMaterialGroupsAsync()
    {
        var response = await SendAsync(_http.GetAsync("api/materialgroups"));
        return await ReadAsync<List<MaterialGroupDto>>(response) ?? new();
    }

    public async Task<MaterialGroupDto?> GetMaterialGroupAsync(int id)
    {
        try
        {
            var response = await SendAsync(_http.GetAsync($"api/materialgroups/{id}"));
            return await ReadAsync<MaterialGroupDto>(response);
        }
        catch (ApiException ex) when (ex.Kind == ApiErrorKind.NotFound)
        {
            return null;
        }
    }

    public async Task<MaterialGroupDto> CreateMaterialGroupAsync(MaterialGroupEditDto dto)
    {
        var response = await SendAsync(_http.PostAsJsonAsync("api/materialgroups", dto));
        return await ReadAsync<MaterialGroupDto>(response)
               ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул созданную группу.");
    }

    public async Task UpdateMaterialGroupAsync(int id, MaterialGroupEditDto dto)
    {
        await SendAsync(_http.PutAsJsonAsync($"api/materialgroups/{id}", dto));
    }

    public async Task<List<UnitDto>> GetUnitsAsync()
    {
        var response = await SendAsync(_http.GetAsync("api/units"));
        return await ReadAsync<List<UnitDto>>(response) ?? new();
    }

    public async Task<List<MaterialDto>> GetMaterialsAsync(int? groupId = null)
    {
        var response = await SendAsync(_http.GetAsync(groupId.HasValue ? $"api/materials?groupId={groupId}" : "api/materials"));
        return await ReadAsync<List<MaterialDto>>(response) ?? new();
    }

    public async Task<MaterialDto?> GetMaterialAsync(int id)
    {
        try
        {
            var response = await SendAsync(_http.GetAsync($"api/materials/{id}"));
            return await ReadAsync<MaterialDto>(response);
        }
        catch (ApiException ex) when (ex.Kind == ApiErrorKind.NotFound)
        {
            return null;
        }
    }

    public async Task<MaterialDto> CreateMaterialAsync(MaterialEditDto dto)
    {
        var response = await SendAsync(_http.PostAsJsonAsync("api/materials", dto));
        return await ReadAsync<MaterialDto>(response)
               ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул созданный материал.");
    }

    public async Task UpdateMaterialAsync(int id, MaterialEditDto dto)
    {
        await SendAsync(_http.PutAsJsonAsync($"api/materials/{id}", dto));
    }

    public async Task DeleteMaterialAsync(int id)
    {
        await SendAsync(_http.DeleteAsync($"api/materials/{id}"));
    }

    public async Task<PurchaseRequestDto?> GetPurchaseRequestAsync(int id)
    {
        try
        {
            var response = await SendAsync(_http.GetAsync($"api/purchaserequests/{id}"));
            return await ReadAsync<PurchaseRequestDto>(response);
        }
        catch (ApiException ex) when (ex.Kind == ApiErrorKind.NotFound)
        {
            return null;
        }
    }

    public async Task<PurchaseRequestDto> CreatePurchaseRequestAsync(PurchaseRequestEditDto dto)
    {
        var response = await SendAsync(_http.PostAsJsonAsync("api/purchaserequests", dto));
        return await ReadAsync<PurchaseRequestDto>(response)
               ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул созданную заявку.");
    }

    public async Task UpdatePurchaseRequestAsync(int id, PurchaseRequestEditDto dto)
    {
        await SendAsync(_http.PutAsJsonAsync($"api/purchaserequests/{id}", dto));
    }

    public async Task<PurchaseRequestDto> ChangeRequestStatusAsync(int id, int statusId)
    {
        var response = await SendAsync(_http.PostAsJsonAsync(
            $"api/purchaserequests/{id}/status",
            new RequestStatusChangeDto { StatusId = statusId }));

        return await ReadAsync<PurchaseRequestDto>(response)
               ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул обновленную заявку.");
    }

    public async Task<List<RequestStatusDto>> GetRequestStatusesAsync()
    {
        var response = await SendAsync(_http.GetAsync("api/purchaserequests/statuses"));
        return await ReadAsync<List<RequestStatusDto>>(response) ?? new();
    }

    public async Task<List<PurchaseRequestReportRowDto>> GetPurchaseRequestReportAsync(
        PurchaseRequestReportFilterDto filter)
    {
        var response = await SendAsync(_http.PostAsJsonAsync("api/reports/purchase-requests", filter));
        return await ReadAsync<List<PurchaseRequestReportRowDto>>(response) ?? new();
    }
}
