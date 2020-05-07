using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TypeRegistrationSetupResolver : ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig>
    {
        public ITypeRegistration ResolveSetup(CustomTypeRegistrationConfig config, ICollectionSetup? collection = default)
        {
            return new CustomTypeRegistrationSetup
            {
                Type = config.Type == typeof(CollectionConfig) ? typeof(CollectionSetup) : config.Type,
                Alias = config.Alias,
                Parameters = config.Parameters
            };
        }
    }
}
