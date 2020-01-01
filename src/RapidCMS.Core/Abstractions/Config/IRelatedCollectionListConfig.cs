using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IRelatedCollectionListConfig<TEntity, TRelatedEntity>
        where TRelatedEntity : IEntity
        where TEntity : IEntity
    {

    }
}
