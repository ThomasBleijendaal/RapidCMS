using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class EntityVariantSetupResolver : ISetupResolver<EntityVariantSetup, EntityVariantConfig>
    {
        public Task<IResolvedSetup<EntityVariantSetup>> ResolveSetupAsync(EntityVariantConfig config, CollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (config == default)
            {
                return Task.FromResult<IResolvedSetup<EntityVariantSetup>>(new ResolvedSetup<EntityVariantSetup>(EntityVariantSetup.Undefined, true));
            }
            else
            {
                return Task.FromResult<IResolvedSetup<EntityVariantSetup>>(new ResolvedSetup<EntityVariantSetup>(
                    new EntityVariantSetup(config.Name, config.Icon, config.Type, AliasHelper.GetEntityVariantAlias(config.Type)),
                    true));
            }
        }
    }
}
