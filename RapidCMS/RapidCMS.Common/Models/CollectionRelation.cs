using System;
using RapidCMS.Common.Models.Metadata;

#nullable enable

namespace RapidCMS.Common.Models
{
    internal class CollectionRelation : Relation
    {
        internal string CollectionAlias { get; set; }
        internal Type RelatedEntityType { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal IExpressionMetadata DisplayProperty { get; set; }
    }
}
