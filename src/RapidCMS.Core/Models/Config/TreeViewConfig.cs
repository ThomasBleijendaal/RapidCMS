using RapidCMS.Core.Enums;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Models.Config
{
    internal class TreeViewConfig
    {
        internal EntityVisibilty EntityVisibilty { get; set; }
        internal CollectionRootVisibility RootVisibility { get; set; }
        internal IExpressionMetadata? Name { get; set; }
    }
}
