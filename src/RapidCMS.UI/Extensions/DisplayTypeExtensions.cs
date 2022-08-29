using System;
using RapidCMS.Core.Enums;
using RapidCMS.UI.Components.Displays;

namespace RapidCMS.UI.Extensions
{
    public static class DisplayTypeExtensions
    {
        public static Type? GetDisplay(this DisplayType displayType)
            => displayType switch
            {
                DisplayType.Label => typeof(LabelDisplay),
                DisplayType.Pre => typeof(PreDisplay),
                DisplayType.Link => typeof(LinkDisplay),
                _ => null
            };
    }
}
