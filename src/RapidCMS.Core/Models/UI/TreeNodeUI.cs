using System.Collections.Generic;

namespace RapidCMS.Core.Models.UI
{
    public class TreeNodeUI
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool RootVisibleOfCollections { get; set; }
        public List<string> Collections { get; set; }
    }
}
