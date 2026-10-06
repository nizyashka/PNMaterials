using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsClient.Navigation;

public abstract class NavItem
{
    public required string Title { get; init; }
}

public sealed class NavFolder : NavItem
{
    public List<NavItem> Children { get; init; } = [];
    public bool IsExpanded { get; init; } = true;
}

public sealed class NavPage : NavItem
{
    public required Type PageType { get; init; }
}
