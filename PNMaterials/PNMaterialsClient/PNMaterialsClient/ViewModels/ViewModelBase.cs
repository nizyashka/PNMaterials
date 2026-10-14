using PNMaterialsClient.Services;

namespace PNMaterialsClient.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    protected ILogger Logger { get; }

    protected ViewModelBase(ILogger logger)
    {
        Logger = logger;
    }

    [ObservableProperty]
    private UiMessage? _message;

    protected void ShowInfo(string text) => Message = new UiMessage(MessageSeverity.Info, text);
    protected void ShowSuccess(string text) => Message = new UiMessage(MessageSeverity.Success, text);
    protected void ShowWarning(string text) => Message = new UiMessage(MessageSeverity.Warning, text);
    protected void ShowError(string text) => Message = new UiMessage(MessageSeverity.Error, text);
    protected void ClearMessage() => Message = null;

    protected async Task RunAsync(string operation, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (ApiException ex) when (ex.Kind is ApiErrorKind.Validation or ApiErrorKind.NotFound or ApiErrorKind.Conflict)
        {
            Logger.LogWarning(ex, "{Operation}: {Kind}", operation, ex.Kind);
            ShowWarning(ex.Message);
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "{Operation}: {Kind}", operation, ex.Kind);
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            ReportError(operation, ex);
        }
    }

    public void ReportError(string operation, Exception exception)
    {
        Logger.LogError(exception, "{Operation}: непредвиденная ошибка", operation);
        ShowError("Непредвиденная ошибка. Подробности записаны в журнал.");
    }
}
