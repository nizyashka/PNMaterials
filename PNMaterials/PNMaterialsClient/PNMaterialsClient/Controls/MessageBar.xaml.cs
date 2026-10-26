using PNMaterialsClient.Services;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Controls;

public sealed partial class MessageBar : UserControl
{
    private readonly DispatcherTimer _hideTimer = new() { Interval = TimeSpan.FromSeconds(3) };

    private MessagesService MessagesService { get; }

    public MessageBar()
    {
        this.InitializeComponent();
        MessagesService = App.Services.GetRequiredService<MessagesService>();
        _hideTimer.Tick += OnHideTimerTick;
        Bar.Closed += OnInfoBarClosed;
    }

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(nameof(Message), typeof(UiMessage), typeof(MessageBar),
            new PropertyMetadata(null, OnMessageChanged));

    public UiMessage? Message
    {
        get => (UiMessage?)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((MessageBar)d).ShowMessage();

    private void ShowMessage()
    {
        _hideTimer.Stop();

        if (Message is null)
        {
            Bar.IsOpen = false;
            return;
        }

        Bar.Severity = Message.Severity switch
        {
            MessageSeverity.Success => InfoBarSeverity.Success,
            MessageSeverity.Warning => InfoBarSeverity.Warning,
            MessageSeverity.Error => InfoBarSeverity.Error,
            _ => InfoBarSeverity.Informational
        };
        Bar.Message = Message.Text;
        Bar.IsOpen = true;

        if (Message.Severity == MessageSeverity.Success || Message.Severity == MessageSeverity.Info)
            _hideTimer.Start();
    }

    private void OnHideTimerTick(object? sender, object e)
    {
        _hideTimer.Stop();
        Bar.IsOpen = false;
    }

    private void OnInfoBarClosed(InfoBar sender, InfoBarClosedEventArgs args)
    {
        _hideTimer.Stop();
        MessagesService.RemoveMessage(Message);
    }
}
