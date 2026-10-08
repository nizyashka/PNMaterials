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

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(EntityListView), new PropertyMetadata(null));
    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(EntityListView), new PropertyMetadata(null));
    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ColumnsHeaderProperty =
        DependencyProperty.Register(nameof(ColumnsHeader), typeof(object), typeof(EntityListView), new PropertyMetadata(null));
    public object? ColumnsHeader
    {
        get => GetValue(ColumnsHeaderProperty);
        set => SetValue(ColumnsHeaderProperty, value);
    }

    public static readonly DependencyProperty IsFilterVisibleProperty =
        DependencyProperty.Register(nameof(IsFilterVisible), typeof(bool), typeof(EntityListView), new PropertyMetadata(false));
    public bool IsFilterVisible
    {
        get => (bool)GetValue(IsFilterVisibleProperty);
        set => SetValue(IsFilterVisibleProperty, value);
    }

    public event RoutedEventHandler? CreateClicked;
    private void OnCreateButtonClick(object sender, RoutedEventArgs e)
        => CreateClicked?.Invoke(this, e);

    public event RoutedEventHandler? FilterClicked;
    private void OnFilterButtonClick(object sender, RoutedEventArgs e)
        => FilterClicked?.Invoke(this, e);

    public event ItemClickEventHandler? ItemClicked;
    private void OnItemClick(object sender, ItemClickEventArgs e)
        => ItemClicked?.Invoke(this, e);

    public EntityListView()
    {
        this.InitializeComponent();
    }
}
