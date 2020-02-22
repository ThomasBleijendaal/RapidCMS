using System;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Models.UI
{
    public class TreePageUI
    {
        public TreePageUI(string name, string icon, PageStateModel state)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            State = state ?? throw new ArgumentNullException(nameof(state));
        }

        public string Name { get; private set; }
        public string Icon { get; private set; }
        public PageStateModel State { get; private set; }
    }
}
