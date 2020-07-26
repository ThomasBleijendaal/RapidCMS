using System.Collections.Generic;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.UI
{
    public class TreeRootUI : TreeNodeUI
    {
        public TreeRootUI(string id, string repositoryAlias, string name, List<(string alias, PageType type)> collections) : base(id, repositoryAlias, name, collections)
        {
        }
    }
}
