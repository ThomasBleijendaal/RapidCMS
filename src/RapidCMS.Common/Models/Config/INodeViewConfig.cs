using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface INodeViewConfig<TEntity>
        : IHasButtons<INodeViewConfig<TEntity>>,
        IHasDisplayPanes<TEntity, INodeViewConfig<TEntity>>
        where TEntity : IEntity
    {

    }
}
