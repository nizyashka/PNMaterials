using Microsoft.UI.Xaml.Markup;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Controls;

[ContentProperty(Name = nameof(Body))]
public sealed partial class EntityCard : UserControl
{
    public EntityCard()
    {
        this.InitializeComponent();
        UpdateEditButtons();
        UpdateArchiveButtons();
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(EntityCard),
            new PropertyMetadata(string.Empty));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty IsEditModeProperty =
        DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(EntityCard),
            new PropertyMetadata(false, OnIsEditModeChanged));

    public bool IsEditMode
    {
        get => (bool)GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    public static readonly DependencyProperty IsArchivedProperty =
        DependencyProperty.Register(nameof(IsArchived), typeof(bool), typeof(EntityCard),
            new PropertyMetadata(false, OnIsArchivedStatusChanged));

    public bool IsArchived
    {
        get => (bool)GetValue(IsArchivedProperty);
        set => SetValue(IsArchivedProperty, value);
    }

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(nameof(Message), typeof(UiMessage), typeof(EntityCard),
            new PropertyMetadata(null));

    public UiMessage? Message
    {
        get => (UiMessage?)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly DependencyProperty BodyProperty =
        DependencyProperty.Register(nameof(Body), typeof(object), typeof(EntityCard),
            new PropertyMetadata(null));

    public object? Body
    {
        get => GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }

    public static readonly DependencyProperty ExtraButtonsProperty =
        DependencyProperty.Register(nameof(ExtraButtons), typeof(object), typeof(EntityCard),
            new PropertyMetadata(null));

    public object? ExtraButtons
    {
        get => GetValue(ExtraButtonsProperty);
        set => SetValue(ExtraButtonsProperty, value);
    }

    public static readonly DependencyProperty SaveCommandProperty =
        DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(EntityCard),
            new PropertyMetadata(null));

    public ICommand? SaveCommand
    {
        get => (ICommand?)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public static readonly DependencyProperty ArchiveCommandProperty =
        DependencyProperty.Register(nameof(ArchiveCommand), typeof(ICommand), typeof(EntityCard),
            new PropertyMetadata(null, OnArchiveCommandChanged));

    public ICommand? ArchiveCommand
    {
        get => (ICommand?)GetValue(ArchiveCommandProperty);
        set => SetValue(ArchiveCommandProperty, value);
    }

    public static readonly DependencyProperty RefreshCommandProperty =
        DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(EntityCard),
            new PropertyMetadata(null));

    public ICommand? RefreshCommand
    {
        get => (ICommand?)GetValue(RefreshCommandProperty);
        set => SetValue(RefreshCommandProperty, value);
    }

    public static readonly DependencyProperty ExitCommandProperty =
        DependencyProperty.Register(nameof(ExitCommand), typeof(ICommand), typeof(EntityCard),
            new PropertyMetadata(null));

    public ICommand? ExitCommand
    {
        get => (ICommand?)GetValue(ExitCommandProperty);
        set => SetValue(ExitCommandProperty, value);
    }

    private static void OnIsEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((EntityCard)d).UpdateEditButtons();

    private void UpdateEditButtons()
    {
        EditButton.Visibility = IsEditMode ? Visibility.Collapsed : Visibility.Visible;
        SaveButton.Visibility = IsEditMode ? Visibility.Visible : Visibility.Collapsed;
    }

    private static void OnIsArchivedStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((EntityCard)d).UpdateArchiveButtons();

    private static void OnArchiveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((EntityCard)d).UpdateArchiveButtons();

    private void UpdateArchiveButtons()
    {
        ArchiveIcon.Glyph = IsArchived ? "\uE7A7" : "\uE74D";
        ToolTipService.SetToolTip(ArchiveButton, IsArchived ? "Восстановить из архива" : "Архивировать");
        ArchiveButton.Visibility = ArchiveCommand is null ? Visibility.Collapsed : Visibility.Visible;
        EditButton.IsEnabled = !IsArchived;
    }

    private void OnToggleEdit(object sender, RoutedEventArgs e)
        => IsEditMode = true;
}
