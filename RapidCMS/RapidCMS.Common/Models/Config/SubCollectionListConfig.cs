using System;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public class SubCollectionListConfig
    {
        public SubCollectionListConfig(string collectionAlias)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        }

        internal int Index { get; set; }

        internal string CollectionAlias { get; set; }
    }

    public class SubCollectionListConfig<TSubEntity> : SubCollectionListConfig
        where TSubEntity : IEntity
    {
        public SubCollectionListConfig(string collectionAlias) : base(collectionAlias)
        {
        }
    }
}
