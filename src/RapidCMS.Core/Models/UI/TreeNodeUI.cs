using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Models.UI
{
    public class TreeNodeUI
    {
        public TreeNodeUI(string id, string name, List<string> collections)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Collections = collections ?? throw new ArgumentNullException(nameof(collections));
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string? Path { get; internal set; }
        public bool RootVisibleOfCollections { get; internal set; }
        public List<string> Collections { get; private set; }
    }
}
