namespace PNMaterialsClient.Controls;

public static class TextBoxCounter
{
    public static readonly DependencyProperty CharCounterProperty = DependencyProperty.RegisterAttached(
        "CharCounter",
        typeof(bool),
        typeof(TextBoxCounter),
        new PropertyMetadata(false, OnCharCounterChanged));

    public static void SetCharCounter(TextBox element, bool value)
    {
        element.SetValue(CharCounterProperty, value);
    }
    public static bool GetCharCounter(TextBox element)
    {
        return (bool)element.GetValue(CharCounterProperty);
    }

    private static void OnCharCounterChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is TextBox textBox)
        {
            if (args.NewValue is true)
            {
                TextBlock textBlock = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Text = $"{textBox.Text.Length}/{textBox.MaxLength}"
                };
                textBox.TextChanged += RefreshCounter;
                textBox.Description = textBlock;
            }
            else
            {
                textBox.TextChanged -= RefreshCounter;
                textBox.Description = null;
            }
        }
    }

    private static void RefreshCounter(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
            if (textBox.Description is TextBlock textBlock)
                textBlock.Text = $"{textBox.Text.Length}/{textBox.MaxLength}";
    }
}
