using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    internal class TreeViewConfig
    {
        internal EntityVisibilty EntityVisibilty { get; set; }
        internal CollectionRootVisibility RootVisibility { get; set; }
        internal IExpressionMetadata? Name { get; set; }
    }
}
