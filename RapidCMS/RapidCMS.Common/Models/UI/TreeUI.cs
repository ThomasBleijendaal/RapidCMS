using System.Collections.Generic;

#nullable enable

namespace RapidCMS.Common.Models.UI
{
    public class TreeUI
    {
        public string Alias { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
        public string? Path { get; set; }
        public List<TreeNodeUI> Nodes { get; set; }

        public bool EntitiesVisible { get; set; }
        public bool RootVisible { get; set; }
    }

    public class TreeNodeUI
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> Collections { get; set; }
    }
    public class TreeRootUI : TreeNodeUI
    {
    }

}
