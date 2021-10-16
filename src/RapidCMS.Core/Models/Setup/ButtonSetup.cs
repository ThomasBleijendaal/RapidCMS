using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup
{
    internal class ButtonSetup : IButtonSetup
    {
        public DefaultButtonType DefaultButtonType { get; internal set; }

        public string ButtonId { get; internal set; } = default!;
        public Type? CustomType { get; internal set; }
        public Type ButtonHandlerType { get; internal set; } = default!;

        public string Label { get; internal set; } = default!;
        public string Icon { get; internal set; } = default!;
        public bool IsPrimary { get; internal set; }

        public Func<object, EntityState, bool>? IsVisible { get; internal set; }

        public IEnumerable<IButtonSetup> Buttons { get; internal set; } = default!;

        public IEntityVariantSetup? EntityVariant { get; internal set; }

        DefaultButtonType IButton.DefaultButtonType => DefaultButtonType;
        string IButton.Label => Label;
        string IButton.Icon => Icon;
        IEntityVariantSetup? IButton.EntityVariant => EntityVariant;
    }
}
