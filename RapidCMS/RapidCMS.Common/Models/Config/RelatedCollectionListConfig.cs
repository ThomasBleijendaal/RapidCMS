using System;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public class RelatedCollectionListConfig
    {
        public RelatedCollectionListConfig(string collectionAlias)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        }

        internal int Index { get; set; }

        internal string CollectionAlias { get; set; }
    }

    public class RelatedCollectionListConfig<TEntity, TRelatedEntity> : RelatedCollectionListConfig
        where TRelatedEntity : IEntity
        where TEntity : IEntity
    {
        public RelatedCollectionListConfig(string collectionAlias) : base(collectionAlias)
        {
        }
    }
}
