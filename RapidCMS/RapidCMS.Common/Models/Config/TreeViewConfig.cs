using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    public class TreeViewConfig
    {
        internal EntityVisibilty EntityVisibilty { get; set; }
        internal CollectionRootVisibility RootVisibility { get; set; }
        internal IExpressionMetadata Name { get; set; }
    }
}
