using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class EntityVariantSetupResolver : ISetupResolver<IEntityVariantSetup, EntityVariantConfig>
    {
        public IEntityVariantSetup ResolveSetup(EntityVariantConfig config, ICollectionSetup collection)
        {
            if (config == default)
            {
                return EntityVariantSetup.Undefined;
            } 
            else
            {
                return new EntityVariantSetup(config.Name, config.Icon, config.Type, config.Type.Name.ToUrlFriendlyString());
            }
        }
    }
}
