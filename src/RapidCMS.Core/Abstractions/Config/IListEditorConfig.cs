using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

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
        /// <summary>
        /// Controls how the ListEditor is displayed: each entity as a row in a table, or as a block in a list
        /// </summary>
        /// <param name="listType"></param>
        /// <returns></returns>
        IListEditorConfig<TEntity> SetListType(ListType listType);

        /// <summary>
        /// Controls whether empty columns in the table should be collapsed. Only required when the
        /// collection uses multiple EntityVariants, with seperate sets of properties which are not shared between the variants. Collapsing
        /// the empty cell will reduce the number of columns required, and makes the table more readable.
        /// </summary>
        /// <param name="columnVisibility"></param>
        /// <returns></returns>
        IListEditorConfig<TEntity> SetColumnVisibility(EmptyVariantColumnVisibility columnVisibility);
    }
}
