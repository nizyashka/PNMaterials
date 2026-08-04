namespace PNMaterialsClient.ViewModels;

public class StatusActionViewModel
{
    public StatusActionViewModel(string text, IRelayCommand command)
    {
        Text = text;
        Command = command;
    }

    public string Text { get; }
    public IRelayCommand Command { get; }
}
