namespace RapidCMS.Core.Abstractions.Config
{
    public interface IOrderByConfig<TEntity> : 
        IHasOrderByEntity<TEntity, IOrderByConfig<TEntity>>,
        IHasOrderByDatabaseEntity<IOrderByConfig<TEntity>>
    {

    }
}
