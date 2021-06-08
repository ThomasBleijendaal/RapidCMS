using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface ITreeViewSetup
    {
        EntityVisibilty EntityVisibility { get; }
        CollectionRootVisibility RootVisibility { get; }
        bool DefaultOpenEntities { get; }
        bool DefaultOpenCollections { get; }

        IExpressionMetadata? Name { get; }
    }
}
