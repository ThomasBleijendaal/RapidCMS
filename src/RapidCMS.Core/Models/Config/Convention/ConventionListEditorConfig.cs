using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionListEditorConfig<TEntity> : ListConfig, IIsConventionBased
    {
        public ConventionListEditorConfig(ListType type) : base(typeof(TEntity))
        {
            Type = type;
        }

        public ListType Type { get; }

        public Features GetFeatures()
        {
            return Features.CanEdit | ((Type == ListType.Block) ? Features.IsBlockList : Features.None);
        }
    }
}
