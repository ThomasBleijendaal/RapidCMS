using RapidCMS.Core.Interfaces.Data;

namespace RapidCMS.Core.Interfaces.Config
{
    public interface IRelatedCollectionListConfig<TEntity, TRelatedEntity>
        where TRelatedEntity : IEntity
        where TEntity : IEntity
    {

    }
}
