using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Models.Config;

internal class ReferencedCollectionConfig : CollectionConfig
{
    internal ReferencedCollectionConfig(string alias) 
        : base(alias, default, default, default, "reference", typeof(IRepository), new EntityVariantConfig("", typeof(IEntity)))
    {
    }
}
