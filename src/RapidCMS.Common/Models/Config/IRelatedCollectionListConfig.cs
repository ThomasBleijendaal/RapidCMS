using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface IRelatedCollectionListConfig<TEntity, TRelatedEntity>
        where TRelatedEntity : IEntity
        where TEntity : IEntity
    {

    }
}
