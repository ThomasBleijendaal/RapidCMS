using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config;

internal class TreeViewConfig
{
    internal EntityVisibilty EntityVisibilty { get; set; }
    internal CollectionRootVisibility RootVisibility { get; set; }
    internal bool DefaultOpenEntities { get; set; }
    internal bool DefaultOpenCollections { get; set; }
    internal IExpressionMetadata? Name { get; set; }
}
