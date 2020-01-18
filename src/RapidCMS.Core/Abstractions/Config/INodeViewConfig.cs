using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface INodeViewConfig<TEntity>
        : IHasButtons<INodeViewConfig<TEntity>>,
        IHasDisplayPanes<TEntity, INodeViewConfig<TEntity>>
        where TEntity : IEntity
    {

    }
}
