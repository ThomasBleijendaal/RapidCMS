using System;

namespace RapidCMS.Core.Models.UI
{
    public class TreeUI
    {
        public TreeUI(string alias, string name)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Alias { get; private set; }
        public string Name { get; private set; }
        public string? Icon { get; internal set; }
        public string? Path { get; internal set; }

        public bool EntitiesVisible { get; internal set; }
        public bool RootVisible { get; internal set; }
    }
}
