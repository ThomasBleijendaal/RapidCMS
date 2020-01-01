using System;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Config
{
    internal class RelatedCollectionListConfig
    {
        protected RelatedCollectionListConfig(string collectionAlias)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        }

        internal int Index { get; set; }

        internal string CollectionAlias { get; set; }
    }

    internal class RelatedCollectionListConfig<TEntity, TRelatedEntity> : RelatedCollectionListConfig, IRelatedCollectionListConfig<TEntity, TRelatedEntity>
        where TRelatedEntity : IEntity
        where TEntity : IEntity
    {
        internal RelatedCollectionListConfig(string collectionAlias) : base(collectionAlias)
        {
        }
    }
}
