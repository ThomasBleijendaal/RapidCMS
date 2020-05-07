using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionListView<TEntity> : ListConfig
        where TEntity : IEntity
    {
        public ConventionListView(bool canGoToNodeEditor)
        {
            CanEdit = canGoToNodeEditor;
        }

        public bool CanEdit { get; }
    }
}
