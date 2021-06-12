using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class ElementSetupResolver : ISetupResolver<IElementSetup, ElementConfig>
    {
        public Task<IResolvedSetup<IElementSetup>> ResolveSetupAsync(ElementConfig config, ICollectionSetup? collection = default)
        {
            return Task.FromResult<IResolvedSetup<IElementSetup>>(new ResolvedSetup<IElementSetup>(new ElementSetup(
                config.IdProperty,
                config.DisplayProperties),
                true));
        }
    }
}
