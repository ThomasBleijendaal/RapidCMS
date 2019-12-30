using RapidCMS.Core.Interfaces.Data;

namespace RapidCMS.Core.Interfaces.Config
{
    public interface INodeViewConfig<TEntity>
        : IHasButtons<INodeViewConfig<TEntity>>,
        IHasDisplayPanes<TEntity, INodeViewConfig<TEntity>>
        where TEntity : IEntity
    {

    }
}
