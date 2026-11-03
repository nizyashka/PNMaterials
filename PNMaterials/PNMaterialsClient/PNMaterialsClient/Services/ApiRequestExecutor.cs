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
            ApiErrorKind.Unauthorized =>
                ExtractMessage(await response.Content.ReadAsStringAsync(), response.StatusCode, "Не удалось подтвердить вход. Войдите в систему повторно."),
            ApiErrorKind.Forbidden =>
                ExtractMessage(await response.Content.ReadAsStringAsync(), response.StatusCode, "Доступ запрещён."),
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
        HttpStatusCode.Unauthorized => ApiErrorKind.Unauthorized,
        HttpStatusCode.Forbidden => ApiErrorKind.Forbidden,
        _ when (int)status >= 500 => ApiErrorKind.Server,
        _ => ApiErrorKind.Unexpected
    };

    private static string ExtractMessage(string body, HttpStatusCode status, string? fallbackMessage = null)
    {
        fallbackMessage ??= $"Ошибка запроса ({(int)status}).";

        if (string.IsNullOrWhiteSpace(body))
            return fallbackMessage;

        try
        {
            using var doc = JsonDocument.Parse(body);

            if (doc.RootElement.TryGetProperty("errors", out var errors))
            {
                var messages = errors.EnumerateObject()
                    .SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString()))
                    .Where(m => !string.IsNullOrWhiteSpace(m));

                var message = string.Join(Environment.NewLine, messages);

                if (!string.IsNullOrWhiteSpace(message))
                    return message;
            }

            if (doc.RootElement.TryGetProperty("detail", out var detail))
            {
                var message = detail.GetString();

                if (!string.IsNullOrWhiteSpace(message))
                    return message;
            }
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException) { }

        return fallbackMessage;
    }
}
