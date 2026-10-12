namespace PNMaterialsClient.Controls;

public sealed partial class EntityListView : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(EntityListView), new PropertyMetadata(string.Empty));
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(string), typeof(EntityListView), new PropertyMetadata(null));
    public string? Status
    {
        get => (string?)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly DependencyProperty TableProperty =
        DependencyProperty.Register(nameof(Table), typeof(object), typeof(EntityListView), new PropertyMetadata(null));
    public object? Table
    {
        get => GetValue(TableProperty);
        set => SetValue(TableProperty, value);
    }

    public event RoutedEventHandler? CreateClicked;
    private void OnCreateButtonClick(object sender, RoutedEventArgs e)
        => CreateClicked?.Invoke(this, e);

    public EntityListView()
    {
        this.InitializeComponent();
    }
}
