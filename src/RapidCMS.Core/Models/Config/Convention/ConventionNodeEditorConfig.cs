using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionNodeEditorConfig<TEntity> : NodeConfig, IIsConventionBased
        where TEntity : IEntity
    {
        public ConventionNodeEditorConfig() : base(typeof(TEntity))
        {
        }

        public Features GetFeatures()
        {
            return Features.CanEdit;
        }
    }
}
