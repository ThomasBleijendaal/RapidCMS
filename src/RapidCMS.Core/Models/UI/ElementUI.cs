using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.UI
{
    public class ElementUI
    {
        internal ElementUI(Func<object, EntityState, bool> isVisible, Func<object, EntityState, bool> isDisabled)
        {
            IsVisible = isVisible ?? throw new ArgumentNullException(nameof(isVisible));
            IsDisabled = isDisabled ?? throw new ArgumentNullException(nameof(isDisabled));
        }

        public Func<object, EntityState, bool> IsVisible { get; private set; }
        public Func<object, EntityState, bool> IsDisabled { get; private set; }
    }
}
