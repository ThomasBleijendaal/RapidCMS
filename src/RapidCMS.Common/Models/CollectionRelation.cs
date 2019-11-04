using System;
using System.Collections.Generic;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class CollectionRelation : Relation
    {
        internal string CollectionAlias { get; set; }
        internal Type RelatedEntityType { get; set; }
        internal IPropertyMetadata? RelatedElementsGetter { get; set; }
        internal IPropertyMetadata? RepositoryParentSelector { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal List<IExpressionMetadata> DisplayProperties { get; set; }
    }
}
