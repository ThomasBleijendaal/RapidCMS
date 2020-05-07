using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionNodeEditor<TEntity> : NodeConfig
        where TEntity : IEntity
    {
        public ConventionNodeEditor() : base(typeof(TEntity))
        {
        }
    }
}
