using RapidCMS.Core.Interfaces.Data;

namespace RapidCMS.Core.Interfaces.Config
{
    public interface INodeEditorConfig<TEntity>
        : IHasButtons<INodeEditorConfig<TEntity>>,
        IHasEditorPanes<TEntity, INodeEditorConfig<TEntity>>
        where TEntity : IEntity
    {

    }
}
