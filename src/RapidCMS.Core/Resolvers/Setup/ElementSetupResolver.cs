using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class ElementSetupResolver : ISetupResolver<ElementSetup, ElementConfig>
    {
        public Task<IResolvedSetup<ElementSetup>> ResolveSetupAsync(ElementConfig config, CollectionSetup? collection = default)
        {
            return Task.FromResult<IResolvedSetup<ElementSetup>>(new ResolvedSetup<ElementSetup>(new ElementSetup(
                config.IdProperty,
                config.DisplayProperties),
                true));
        }
    }
}
