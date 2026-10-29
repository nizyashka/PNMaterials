using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PNMaterialsClient.Services;

public static class ApiRequestExecutor
{
    public static async Task<HttpResponseMessage> SendAsync(Task<HttpResponseMessage> request)
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

    public static async Task<T?> ReadAsync<T>(HttpResponseMessage response)
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
}
