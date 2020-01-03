using System.Collections.Generic;

namespace RapidCMS.Core.Models.UI
{
    public class TreeRootUI : TreeNodeUI
    {
        public TreeRootUI(string id, string name, List<string> collections) : base(id, name, collections)
        {
        }
    }
}
