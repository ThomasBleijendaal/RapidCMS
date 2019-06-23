using System;
using System.Collections.Generic;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models.Metadata;


namespace RapidCMS.Common.Models
{
    internal class CollectionRelation : Relation
    {
        internal string CollectionAlias { get; set; }
        internal Type RelatedEntityType { get; set; }
        internal IPropertyMetadata? RepositoryParentIdProperty { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal List<IExpressionMetadata> DisplayProperties { get; set; }

        internal Func<IEntity, IEnumerable<IRelatedElement>, IEnumerable<string>?>? ValidationFunction { get; set; }
    }
}
