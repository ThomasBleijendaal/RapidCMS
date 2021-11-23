using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup
{
    public class ButtonSetup : IButtonSetup
    {
        public DefaultButtonType DefaultButtonType { get; set; }

        public string ButtonId { get; set; } = default!;
        public Type? CustomType { get; set; }
        public Type ButtonHandlerType { get; set; } = default!;

        public string Label { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public bool IsPrimary { get; set; }

        public Func<object, EntityState, bool>? IsVisible { get; set; }

        public IEnumerable<IButtonSetup> Buttons { get; set; } = default!;

        public IEntityVariantSetup? EntityVariant { get; set; }

        DefaultButtonType IButton.DefaultButtonType => DefaultButtonType;
        string IButton.Label => Label;
        string IButton.Icon => Icon;
        IEntityVariantSetup? IButton.EntityVariant => EntityVariant;
    }
}
