using System;
using System.Collections.Generic;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.UI;

public class SectionUI
{
    public SectionUI(Func<object, EntityState, bool> isVisible)
    {
        IsVisible = isVisible ?? throw new ArgumentNullException(nameof(isVisible));
    }

    public List<ButtonUI>? Buttons { get; internal set; }

    public List<ElementUI>? Elements { get; internal set; }
    public Func<object, EntityState, bool> IsVisible { get; private set; }
    public string? Label { get; internal set; }

    public Type? CustomType { get; internal set; }
}
