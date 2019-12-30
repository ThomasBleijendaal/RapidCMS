using RapidCMS.Core.Enums;
using RapidCMS.Core.Interfaces.Metadata;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class TreeViewSetup
    {
        internal TreeViewSetup(TreeViewConfig treeView)
        {
            EntityVisibility = treeView.EntityVisibilty;
            RootVisibility = treeView.RootVisibility;
            Name = treeView.Name;

        }

        internal EntityVisibilty EntityVisibility { get; set; }
        internal CollectionRootVisibility RootVisibility { get; set; }

        internal IExpressionMetadata? Name { get; set; }
    }
}
