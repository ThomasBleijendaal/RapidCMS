using System;

namespace RapidCMS.Common.Models.UI
{
    public class ElementUI
    {
        public Func<object, bool> IsVisible { get; internal set; }
    }
}
