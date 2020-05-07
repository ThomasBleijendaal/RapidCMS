using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionListViewConfig<TEntity> : ListConfig, IIsConventionBased
        where TEntity : IEntity
    {
        public ConventionListViewConfig(bool canGoToNodeEditor)
        {
            CanEdit = canGoToNodeEditor;
        }

        public bool CanEdit { get; }
    }
}
