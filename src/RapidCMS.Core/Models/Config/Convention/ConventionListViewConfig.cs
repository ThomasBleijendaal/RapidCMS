using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionListViewConfig<TEntity> : ListConfig, IIsConventionBased
        where TEntity : IEntity
    {
        public ConventionListViewConfig(bool canGoToNodeEditor) : base(typeof(TEntity))
        {
            CanGoToNodeEditor = canGoToNodeEditor;
        }

        public bool CanGoToNodeEditor { get; }

        public Features GetFeatures()
        {
            return CanGoToNodeEditor ? Features.CanGoToEdit : Features.None;
        }   
    }
}
