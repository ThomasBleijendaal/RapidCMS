using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IButton
    {
        DefaultButtonType DefaultButtonType { get; }

        string Label { get; }
        string Icon { get; }

        IEntityVariantSetup? EntityVariant { get; }

        Func<object, EntityState, bool>? IsVisible { get; }
    }
}
