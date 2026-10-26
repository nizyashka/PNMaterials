using PNMaterialsClient.Services;

namespace PNMaterialsClient.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    protected ILogger Logger { get; }
    protected MessagesService MessagesService { get; }

    protected ViewModelBase(ILogger logger)
    {
        Logger = logger;
        MessagesService = App.Services.GetRequiredService<MessagesService>();
    }

    protected async Task RunAsync(string operation, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (ApiException ex) when (ex.Kind is ApiErrorKind.Validation or ApiErrorKind.NotFound or ApiErrorKind.Conflict)
        {
            Logger.LogWarning(ex, "{Operation}: {Kind}, ответ сервера: {StatusCode}", operation, ex.Kind.ToString(), FormatStatusCode(ex));
            MessagesService.ShowWarning(ex.Message);
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "{Operation}: {Kind}, ответ сервера: {StatusCode}", operation, ex.Kind.ToString(), FormatStatusCode(ex));
            MessagesService.ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            ReportError(operation, ex);
        }
    }

    private static string FormatStatusCode(ApiException ex)
        => ex.StatusCode is { } code ? ((int)code).ToString() : "нет";

    public void ReportError(string operation, Exception exception)
    {
        Logger.LogError(exception, "{Operation}: непредвиденная ошибка", operation);
        MessagesService.ShowError("Непредвиденная ошибка. Подробности записаны в журнал.");
    }
}
