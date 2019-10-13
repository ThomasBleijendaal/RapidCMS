using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface IListEditorConfig<TEntity>
        : IHasButtons<IListEditorConfig<TEntity>>,
        IHasPageSize<IListEditorConfig<TEntity>>,
        IHasSearchBar<IListEditorConfig<TEntity>>,
        IHasEditorPanes<TEntity, IListEditorConfig<TEntity>>
        where TEntity : IEntity
    {
    }
}
