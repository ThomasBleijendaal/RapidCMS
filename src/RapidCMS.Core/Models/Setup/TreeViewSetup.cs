using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup
{
    internal class TreeViewSetup
    {
        public TreeViewSetup(EntityVisibilty entityVisibility, CollectionRootVisibility rootVisibility, bool defaultOpenEntities, bool defaultOpenCollections, IExpressionMetadata? name)
        {
            EntityVisibility = entityVisibility;
            RootVisibility = rootVisibility;
            DefaultOpenEntities = defaultOpenEntities;
            DefaultOpenCollections = defaultOpenCollections;
            Name = name;
        }

        internal EntityVisibilty EntityVisibility { get; set; }
        internal CollectionRootVisibility RootVisibility { get; set; }
        internal bool DefaultOpenEntities { get; set; }
        internal bool DefaultOpenCollections { get; set; }

        internal IExpressionMetadata? Name { get; set; }
    }
}
