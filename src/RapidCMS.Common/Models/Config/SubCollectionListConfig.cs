using System;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    internal class SubCollectionListConfig
    {
        protected SubCollectionListConfig(string collectionAlias)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        }

        internal int Index { get; set; }

        internal string CollectionAlias { get; set; }
    }

    internal class SubCollectionListConfig<TSubEntity> : SubCollectionListConfig, ISubCollectionListConfig<TSubEntity>
        where TSubEntity : IEntity
    {
        internal SubCollectionListConfig(string collectionAlias) : base(collectionAlias)
        {
        }
    }
}
