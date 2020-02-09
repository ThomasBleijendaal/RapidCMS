using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
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
            DefaultOpenEntities = treeView.DefaultOpenEntities;
            DefaultOpenCollections = treeView.DefaultOpenCollections;
        }

        internal EntityVisibilty EntityVisibility { get; set; }
        internal CollectionRootVisibility RootVisibility { get; set; }
        internal bool DefaultOpenEntities { get; set; }
        internal bool DefaultOpenCollections { get; set; }

        internal IExpressionMetadata? Name { get; set; }
    }
}
