using System.Collections.ObjectModel;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Services;

public class MessagesService
{
    public readonly ObservableCollection<UiMessage> Messages = new ObservableCollection<UiMessage>();

    public void ShowInfo(string text) => Messages.Add(new UiMessage(MessageSeverity.Info, text));
    public void ShowSuccess(string text) => Messages.Add(new UiMessage(MessageSeverity.Success, text));
    public void ShowWarning(string text) => Messages.Add(new UiMessage(MessageSeverity.Warning, text));
    public void ShowError(string text) => Messages.Add(new UiMessage(MessageSeverity.Error, text));
    public void ClearMessages() => Messages.Clear();

    public void RemoveMessage(UiMessage? message)
    {
        Messages.Remove(message);
    }
}
