using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PNMaterialsClient.ViewModels;
using Tmds.DBus.Protocol;
using static System.Net.Mime.MediaTypeNames;

namespace PNMaterialsClient.Services;

public class MessagesService
{
    public ObservableCollection<UiMessage> Messages = new ObservableCollection<UiMessage>() { new UiMessage(MessageSeverity.Info, "Блаблабла"), new UiMessage(MessageSeverity.Warning, "Yapyapyap") };

    public void ShowInfo(string text) => Messages.Add(new UiMessage(MessageSeverity.Info, text));
    public void ShowSuccess(string text) => Messages.Add(new UiMessage(MessageSeverity.Success, text));
    public void ShowWarning(string text) => Messages.Add(new UiMessage(MessageSeverity.Warning, text));
    public void ShowError(string text) => Messages.Add(new UiMessage(MessageSeverity.Error, text));
    public void ClearMessage() => Messages.Clear();
}
