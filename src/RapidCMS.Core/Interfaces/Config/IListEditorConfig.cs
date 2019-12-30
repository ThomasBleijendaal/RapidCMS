using RapidCMS.Core.Interfaces.Data;

namespace RapidCMS.Core.Interfaces.Config
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
