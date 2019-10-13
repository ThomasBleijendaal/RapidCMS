using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
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
