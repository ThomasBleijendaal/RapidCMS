using System;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class FieldSetupResolver : ISetupResolver<FieldSetup, FieldConfig>
    {
        public IResolvedSetup<FieldSetup> ResolveSetup(FieldConfig config, ICollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            // TODO: move logic out of constructors?
            return new ResolvedSetup<FieldSetup>(config switch
            {
                _ when config.EditorType == EditorType.Custom && config.Property != null => new CustomPropertyFieldSetup(config, config.CustomType!),
                _ when config.EditorType != EditorType.None && config.Property != null => new PropertyFieldSetup(config),
                _ when config.DisplayType != DisplayType.None && config.Property != null => new ExpressionFieldSetup(config, config.Property),
                _ when config.DisplayType == DisplayType.Custom && config.Expression != null => new CustomExpressionFieldSetup(config, config.Expression, config.CustomType!),
                _ when config.DisplayType != DisplayType.None && config.Expression != null => new ExpressionFieldSetup(config, config.Expression),
                _ => throw new InvalidOperationException()
            }, true);
        }
    }
}
