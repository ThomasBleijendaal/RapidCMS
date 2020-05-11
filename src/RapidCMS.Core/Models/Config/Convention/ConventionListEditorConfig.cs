using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionListEditorConfig<TEntity> : ListConfig, IIsConventionBased
    {
        public ConventionListEditorConfig() : base(typeof(TEntity))
        {
        }

        public Features GetFeatures()
        {
            return Features.CanEdit;
        }
    }
}
