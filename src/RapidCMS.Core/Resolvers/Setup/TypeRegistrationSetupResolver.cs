using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TypeRegistrationSetupResolver : ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig>
    {
        public Task<IResolvedSetup<ITypeRegistration>> ResolveSetupAsync(CustomTypeRegistrationConfig config, CollectionSetup? collection = default)
        {
            return Task.FromResult<IResolvedSetup<ITypeRegistration>>(
                new ResolvedSetup<ITypeRegistration>(
                    new CustomTypeRegistrationSetup
                    {
                        Type = config.Type == typeof(CollectionConfig) ? typeof(CollectionSetup) : config.Type,
                        Alias = config.Alias,
                        Parameters = config.Parameters
                    },
                    true));
        }
    }
}
