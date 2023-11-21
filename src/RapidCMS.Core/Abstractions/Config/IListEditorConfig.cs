using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Config;

public interface IListEditorConfig<TEntity>
    : IHasButtons<IListEditorConfig<TEntity>>,
    IHasPageSize<IListEditorConfig<TEntity>>,
    IHasSearchBar<IListEditorConfig<TEntity>>,
    IHasEditorPanes<TEntity, IListEditorConfig<TEntity>>,
    ISupportReordering<IListEditorConfig<TEntity>>,
    IHasColumnVisibility<IListEditorConfig<TEntity>>
    where TEntity : IEntity
{
    /// <summary>
    /// Controls how the ListEditor is displayed: each entity as a row in a table, or as a block in a list
    /// </summary>
    /// <param name="listType"></param>
    /// <returns></returns>
    IListEditorConfig<TEntity> SetListType(ListType listType);
}
