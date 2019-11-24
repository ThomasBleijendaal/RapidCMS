using System;
using System.Collections.Generic;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class CollectionRelation : Relation
    {
        public CollectionRelation(
            string collectionAlias, 
            Type relatedEntityType, 
            IPropertyMetadata idProperty, 
            List<IExpressionMetadata> displayProperties)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
            RelatedEntityType = relatedEntityType ?? throw new ArgumentNullException(nameof(relatedEntityType));
            IdProperty = idProperty ?? throw new ArgumentNullException(nameof(idProperty));
            DisplayProperties = displayProperties ?? throw new ArgumentNullException(nameof(displayProperties));
        }

        internal string CollectionAlias { get; set; }
        internal Type RelatedEntityType { get; set; }
        internal IPropertyMetadata? RelatedElementsGetter { get; set; }
        internal IPropertyMetadata? RepositoryParentSelector { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal List<IExpressionMetadata> DisplayProperties { get; set; }
    }
}
