using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsClient.Navigation;

public sealed class NavItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? FolderTemplate { get; set; }
    public DataTemplate? PageTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item) => item switch
    {
        NavFolder => FolderTemplate!,
        NavPage => PageTemplate!,
        _ => base.SelectTemplateCore(item)
    };
}
