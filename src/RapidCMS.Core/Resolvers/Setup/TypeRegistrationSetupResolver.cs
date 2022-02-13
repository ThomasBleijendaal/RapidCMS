using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TypeRegistrationSetupResolver : ISetupResolver<TypeRegistrationSetup, CustomTypeRegistrationConfig>
    {
        public Task<IResolvedSetup<TypeRegistrationSetup>> ResolveSetupAsync(CustomTypeRegistrationConfig config, CollectionSetup? collection = default)
        {
            return Task.FromResult<IResolvedSetup<TypeRegistrationSetup>>(
                new ResolvedSetup<TypeRegistrationSetup>(
                    new TypeRegistrationSetup
                    {
                        Type = config.Type == typeof(CollectionConfig) ? typeof(CollectionSetup) : config.Type,
                        Alias = config.Alias,
                        Parameters = config.Parameters
                    },
                    true));
        }
    }
}
