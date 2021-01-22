using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface ICollectionDetailPageEditorConfig<TDetailEntity> : INodeEditorConfig<TDetailEntity>
        where TDetailEntity : IEntity
    {

    }
}
