using RapidCMS.Common.Data;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    public class PropertyConfig
    {
        internal string Name { get; set; }
        internal string Description { get; set; }

        internal IExpressionMetadata Property { get; set; }
    }

    public class PropertyConfig<TEntity> : PropertyConfig
        where TEntity : IEntity
    {
        
        public PropertyConfig<TEntity> SetName(string name)
        {
            Name = name;
            return this;
        }
        public PropertyConfig<TEntity> SetDescription(string description)
        {
            Description = description;
            return this;
        }
    }
}
