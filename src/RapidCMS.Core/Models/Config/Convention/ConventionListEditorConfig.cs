using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionListEditorConfig<TEntity> : ListConfig, IIsConventionBased
    {
        public ConventionListEditorConfig(bool canGoToNodeEditor, ListType type) : base(typeof(TEntity))
        {
            Type = type;
            CanGoToNodeEditor = canGoToNodeEditor;
        }

        public ListType Type { get; }

        public bool CanGoToNodeEditor { get; }

        public Features GetFeatures()
        {
            return Features.CanEdit |
                (CanGoToNodeEditor ? Features.CanGoToEdit : Features.None) |
                ((Type == ListType.Block) ? Features.IsBlockList : Features.None);
        }
    }
}
