using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config.Convention;

internal class ConventionNodeViewConfig<TEntity> : NodeConfig, IIsConventionBased
    where TEntity : IEntity
{
    public ConventionNodeViewConfig() : base(typeof(TEntity))
    {
    }

    public Features GetFeatures()
    {
        return Features.CanView;
    }
}
