using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config.Convention;

internal class ConventionListViewConfig<TEntity> : ListConfig, IIsConventionBased
    where TEntity : IEntity
{
    public ConventionListViewConfig(bool canGoToNodeEditor, bool canGoToNodeView) : base(typeof(TEntity))
    {
        CanGoToNodeEditor = canGoToNodeEditor;
        CanGoToNodeView = canGoToNodeView;
    }

    public bool CanGoToNodeEditor { get; }
    public bool CanGoToNodeView { get; }

    public Features GetFeatures()
    {
        return CanGoToNodeEditor ? Features.CanGoToEdit : CanGoToNodeView ? Features.CanGoToView : Features.None;
    }   
}
