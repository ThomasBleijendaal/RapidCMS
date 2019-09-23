using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class TreeViewConfigExtensions
    {
        public static TreeView ToTreeView(this TreeViewConfig treeView)
        {
            return new TreeView
            {
                EntityVisibility = treeView.EntityVisibilty,
                RootVisibility = treeView.RootVisibility,
                Name = treeView.Name
            };
        }
    }
}
