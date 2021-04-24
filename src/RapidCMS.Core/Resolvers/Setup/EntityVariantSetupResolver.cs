using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class EntityVariantSetupResolver : ISetupResolver<IEntityVariantSetup, EntityVariantConfig>
    {
        public Task<IResolvedSetup<IEntityVariantSetup>> ResolveSetupAsync(EntityVariantConfig config, ICollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (config == default)
            {
                return Task.FromResult<IResolvedSetup<IEntityVariantSetup>>(new ResolvedSetup<IEntityVariantSetup>(EntityVariantSetup.Undefined, true));
            }
            else
            {
                return Task.FromResult<IResolvedSetup<IEntityVariantSetup>>(new ResolvedSetup<IEntityVariantSetup>(
                    new EntityVariantSetup(config.Name, config.Icon, config.Type, AliasHelper.GetEntityVariantAlias(config.Type)),
                    true));
            }
        }
    }
}
