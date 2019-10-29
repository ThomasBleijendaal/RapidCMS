using System;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.UI
{
    public class ElementUI
    {
        public Func<object, EntityState, bool> IsVisible { get; internal set; }
        public Func<object, EntityState, bool> IsDisabled { get; internal set; }
    }
}
