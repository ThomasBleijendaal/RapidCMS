using System;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.UI;

public class TreePageUI
{
    public TreePageUI(string name, string icon, string color, NavigationState navigateTo)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Icon = icon ?? throw new ArgumentNullException(nameof(icon));
        Color = color ?? throw new ArgumentNullException(nameof(color));
        NavigateTo = navigateTo ?? throw new ArgumentNullException(nameof(navigateTo));
    }

    public string Name { get; private set; }
    public string Icon { get; private set; }
    public string Color { get; private set; }
    public NavigationState NavigateTo { get; private set; }
}
