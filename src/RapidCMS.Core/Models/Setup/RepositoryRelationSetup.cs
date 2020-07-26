using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Models.Setup
{
    internal class RepositoryRelationSetup : RelationSetup
    {
        internal RepositoryRelationSetup(
            string? repositoryAlias,
            string? collectionAlias,
            Type relatedEntityType,
            IPropertyMetadata idProperty,
            List<IExpressionMetadata> displayProperties)
        {
            CollectionAlias = collectionAlias;
            RepositoryAlias = repositoryAlias;
            RelatedEntityType = relatedEntityType ?? throw new ArgumentNullException(nameof(relatedEntityType));
            IdProperty = idProperty ?? throw new ArgumentNullException(nameof(idProperty));
            DisplayProperties = displayProperties ?? throw new ArgumentNullException(nameof(displayProperties));
        }

        public string? CollectionAlias { get; set; }
        public string? RepositoryAlias { get; set; }
        public Type RelatedEntityType { get; set; }
        public IPropertyMetadata? RelatedElementsGetter { get; set; }
        public IPropertyMetadata? RepositoryParentSelector { get; set; }
        public bool EntityAsParent { get; set; }
        public IPropertyMetadata IdProperty { get; set; }
        public List<IExpressionMetadata> DisplayProperties { get; set; }
    }
}
