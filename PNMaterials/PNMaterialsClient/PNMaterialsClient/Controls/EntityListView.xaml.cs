using PNMaterialsClient.ViewModels;

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

    public static readonly DependencyProperty ExtraButtonsProperty =
        DependencyProperty.Register(nameof(ExtraButtons), typeof(object), typeof(EntityListView), new PropertyMetadata(null));
    public object? ExtraButtons
    {
        get => GetValue(ExtraButtonsProperty);
        set => SetValue(ExtraButtonsProperty, value);
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
