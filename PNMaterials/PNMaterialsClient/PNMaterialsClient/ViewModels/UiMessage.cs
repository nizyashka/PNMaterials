namespace PNMaterialsClient.ViewModels;

public sealed class UiMessage(MessageSeverity severity, string text)
{
    public MessageSeverity Severity { get; } = severity;
    public string Text { get; } = text;
}
