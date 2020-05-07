using RapidCMS.Core.Abstractions.Config;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionListEditorConfig<TEntity> : ListConfig, IIsConventionBased
    {
        public T GenerateConfig<T>() where T : class
        {
            throw new System.NotImplementedException();
        }
    }
}
