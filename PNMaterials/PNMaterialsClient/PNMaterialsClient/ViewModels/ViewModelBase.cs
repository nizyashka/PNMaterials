using PNMaterialsClient.Services;

namespace PNMaterialsClient.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    private readonly LoadingService LoadingService;

    protected ILogger Logger { get; }
    protected MessagesService MessagesService { get; }
    protected readonly AuthService _authService;

    protected ViewModelBase(ILogger logger, AuthService authService)
    {
        Logger = logger;
        MessagesService = App.Services.GetRequiredService<MessagesService>();
        _authService = authService;
        LoadingService = App.Services.GetRequiredService<LoadingService>();
    }

    protected async Task RunAsync(string operation, Func<Task> action)
    {
        try
        {
            LoadingService.StartOperation();
            await action();
        }
        catch (ApiException ex) when (ex.Kind is ApiErrorKind.Validation or ApiErrorKind.NotFound or ApiErrorKind.Conflict or ApiErrorKind.Forbidden)
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
        finally
        {
            LoadingService.EndOperation();
        }
    }

    private static string FormatStatusCode(ApiException ex)
        => ex.StatusCode is { } code ? ((int)code).ToString() : "нет";

    public void ReportError(string operation, Exception exception)
    {
        Logger.LogError(exception, "{Operation}: непредвиденная ошибка", operation);
        MessagesService.ShowError("Непредвиденная ошибка. Подробности записаны в журнал.");
    }

    public bool IsAuthorized(string requiredRole)
    {
        bool result = _authService.IsAuthorized(requiredRole);

        if (!result)
            MessagesService.ShowWarning("У вас недостаточно прав для этого действия.");

        return result;
    }
}
