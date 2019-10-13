using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface INodeEditorConfig<TEntity>
        : IHasButtons<INodeEditorConfig<TEntity>>,
        IHasEditorPanes<TEntity, INodeEditorConfig<TEntity>>
        where TEntity : IEntity
    {

    }
}
