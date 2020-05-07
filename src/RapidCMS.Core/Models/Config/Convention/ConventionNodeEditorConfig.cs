using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionNodeEditorConfig<TEntity> : NodeConfig, IIsConventionBased
        where TEntity : IEntity
    {
        public ConventionNodeEditorConfig() : base(typeof(TEntity))
        {
        }
    }
}
