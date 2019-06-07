using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

#nullable enable

namespace RapidCMS.Common.Models
{
    internal class TreeView
    {
        internal EntityVisibilty EntityVisibility { get; set; }
        internal CollectionRootVisibility RootVisibility { get; set; }

        internal IExpressionMetadata Name { get; set; }
    }
}
