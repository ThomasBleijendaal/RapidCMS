using RapidCMS.Core.Interfaces.Data;

namespace RapidCMS.Core.Interfaces.Config
{
    public interface IListViewConfig<TEntity>
        : IHasButtons<IListViewConfig<TEntity>>,
        IHasPageSize<IListViewConfig<TEntity>>,
        IHasSearchBar<IListViewConfig<TEntity>>,
        IHasDisplayTable<TEntity, IListViewConfig<TEntity>>
        where TEntity : IEntity
    {
    }
}
