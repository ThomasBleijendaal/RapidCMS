using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
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
