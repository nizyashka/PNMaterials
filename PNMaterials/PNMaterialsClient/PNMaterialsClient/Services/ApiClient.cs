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

    public async Task<List<MaterialGroupDto>> GetMaterialGroupsAsync()
    {
        var response = await ApiRequestExecutor.SendAsync(_http.GetAsync("api/materialgroups"));
        return await ApiRequestExecutor.ReadAsync<List<MaterialGroupDto>>(response) ?? new();
    }

    public async Task<MaterialGroupDto?> GetMaterialGroupAsync(int id)
    {
        try
        {
            var response = await ApiRequestExecutor.SendAsync(_http.GetAsync($"api/materialgroups/{id}"));
            return await ApiRequestExecutor.ReadAsync<MaterialGroupDto>(response);
        }
        catch (ApiException ex) when (ex.Kind == ApiErrorKind.NotFound)
        {
            return null;
        }
    }

    public async Task<MaterialGroupDto> CreateMaterialGroupAsync(MaterialGroupEditDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync("api/materialgroups", dto));
        return await ApiRequestExecutor.ReadAsync<MaterialGroupDto>(response)
               ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул созданную группу.");
    }

    public async Task UpdateMaterialGroupAsync(int id, MaterialGroupEditDto dto)
    {
        await ApiRequestExecutor.SendAsync(_http.PutAsJsonAsync($"api/materialgroups/{id}", dto));
    }

    public async Task DeleteMaterialGroupAsync(int id)
    {
        await ApiRequestExecutor.SendAsync(_http.DeleteAsync($"api/materialgroups/{id}"));
    }

    public async Task<List<UnitDto>> GetUnitsAsync()
    {
        var response = await ApiRequestExecutor.SendAsync(_http.GetAsync("api/units"));
        return await ApiRequestExecutor.ReadAsync<List<UnitDto>>(response) ?? new();
    }

    public async Task<List<MaterialDto>> GetMaterialsAsync(int? groupId = null, bool includeArchived = false)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.GetAsync(groupId.HasValue ? $"api/materials?includeArchived={includeArchived}&groupId={groupId}"
            : $"api/materials?includeArchived={includeArchived}"));
        return await ApiRequestExecutor.ReadAsync<List<MaterialDto>>(response) ?? new();
    }

    public async Task<MaterialDto?> GetMaterialAsync(int id)
    {
        try
        {
            var response = await ApiRequestExecutor.SendAsync(_http.GetAsync($"api/materials/{id}"));
            return await ApiRequestExecutor.ReadAsync<MaterialDto>(response);
        }
        catch (ApiException ex) when (ex.Kind == ApiErrorKind.NotFound)
        {
            return null;
        }
    }

    public async Task<MaterialDto> CreateMaterialAsync(MaterialEditDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync("api/materials", dto));
        return await ApiRequestExecutor.ReadAsync<MaterialDto>(response)
               ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул созданный материал.");
    }

    public async Task UpdateMaterialAsync(int id, MaterialEditDto dto)
    {
        await ApiRequestExecutor.SendAsync(_http.PutAsJsonAsync($"api/materials/{id}", dto));
    }

    public async Task<MaterialDto> ArchiveMaterialAsync(int id)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsync($"api/materials/{id}/archive", null));
        return await ApiRequestExecutor.ReadAsync<MaterialDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул архивированный материал.");
    }

    public async Task<MaterialDto> RestoreMaterialAsync(int id)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsync($"api/materials/{id}/restore", null));
        return await ApiRequestExecutor.ReadAsync<MaterialDto>(response)
            ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул восстановленный материал.");
    }

    public async Task DeleteMaterialAsync(int id)
    {
        await ApiRequestExecutor.SendAsync(_http.DeleteAsync($"api/materials/{id}"));
    }

    public async Task<PurchaseRequestDto?> GetPurchaseRequestAsync(int id)
    {
        try
        {
            var response = await ApiRequestExecutor.SendAsync(_http.GetAsync($"api/purchaserequests/{id}"));
            return await ApiRequestExecutor.ReadAsync<PurchaseRequestDto>(response);
        }
        catch (ApiException ex) when (ex.Kind == ApiErrorKind.NotFound)
        {
            return null;
        }
    }

    public async Task<PurchaseRequestDto> CreatePurchaseRequestAsync(PurchaseRequestEditDto dto)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync("api/purchaserequests", dto));
        return await ApiRequestExecutor.ReadAsync<PurchaseRequestDto>(response)
               ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул созданную заявку.");
    }

    public async Task UpdatePurchaseRequestAsync(int id, PurchaseRequestEditDto dto)
    {
        await ApiRequestExecutor.SendAsync(_http.PutAsJsonAsync($"api/purchaserequests/{id}", dto));
    }

    public async Task<PurchaseRequestDto> ChangeRequestStatusAsync(int id, int statusId)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync(
            $"api/purchaserequests/{id}/status",
            new RequestStatusChangeDto { StatusId = statusId }));

        return await ApiRequestExecutor.ReadAsync<PurchaseRequestDto>(response)
               ?? throw new ApiException(ApiErrorKind.InvalidResponse, "Сервер не вернул обновленную заявку.");
    }

    public async Task<List<RequestStatusDto>> GetRequestStatusesAsync()
    {
        var response = await ApiRequestExecutor.SendAsync(_http.GetAsync("api/purchaserequests/statuses"));
        return await ApiRequestExecutor.ReadAsync<List<RequestStatusDto>>(response) ?? new();
    }

    public async Task<List<PurchaseRequestReportRowDto>> GetPurchaseRequestReportAsync(
        PurchaseRequestReportFilterDto filter)
    {
        var response = await ApiRequestExecutor.SendAsync(_http.PostAsJsonAsync("api/reports/purchase-requests", filter));
        return await ApiRequestExecutor.ReadAsync<List<PurchaseRequestReportRowDto>>(response) ?? new();
    }
}
