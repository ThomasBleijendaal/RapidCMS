using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IListEditorConfig<TEntity>
        : IHasButtons<IListEditorConfig<TEntity>>,
        IHasPageSize<IListEditorConfig<TEntity>>,
        IHasSearchBar<IListEditorConfig<TEntity>>,
        IHasEditorPanes<TEntity, IListEditorConfig<TEntity>>,
        ISupportReordering<IListEditorConfig<TEntity>>
        where TEntity : IEntity
    {
    }
}
