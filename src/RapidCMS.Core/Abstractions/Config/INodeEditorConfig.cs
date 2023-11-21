using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config;

public interface INodeEditorConfig<TEntity>
    : IHasButtons<INodeEditorConfig<TEntity>>,
    IHasEditorPanes<TEntity, INodeEditorConfig<TEntity>>
    where TEntity : IEntity
{

}
