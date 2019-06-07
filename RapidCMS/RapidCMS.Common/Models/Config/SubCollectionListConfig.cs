using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public class SubCollectionListConfig
    {
        internal int Index { get; set; }

        internal string CollectionAlias { get; set; }
    }

    public class SubCollectionListConfig<TSubEntity> : SubCollectionListConfig
        where TSubEntity : IEntity
    {
    }
}
