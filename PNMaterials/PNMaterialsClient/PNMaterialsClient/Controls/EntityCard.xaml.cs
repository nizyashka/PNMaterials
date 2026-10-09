using Microsoft.UI.Xaml.Markup;

namespace PNMaterialsClient.Controls;

[ContentProperty(Name = nameof(Body))]
public sealed partial class EntityCard : UserControl
{
    public EntityCard()
    {
        this.InitializeComponent();
        UpdateEditButtons();
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

    private void OnToggleEdit(object sender, RoutedEventArgs e)
        => IsEditMode = true;
}
