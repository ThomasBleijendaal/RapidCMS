using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface ISetupResolver<TSetup>
    {
        Task<TSetup> ResolveSetupAsync();
        Task<TSetup> ResolveSetupAsync(string alias);
    }

    public interface ISetupResolver<TSetup, TConfig>
        where TConfig : notnull
    {
        Task<IResolvedSetup<TSetup>> ResolveSetupAsync(TConfig config, ICollectionSetup? collection = default);
    }
}
