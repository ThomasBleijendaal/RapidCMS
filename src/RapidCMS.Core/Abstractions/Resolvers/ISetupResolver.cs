using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface ISetupResolver<TSetup>
    {
        TSetup ResolveSetup();
        TSetup ResolveSetup(string alias);
    }

    internal interface ISetupResolver<TSetup, TConfig>
        where TConfig : notnull
    {
        TSetup ResolveSetup(TConfig config, ICollectionSetup? collection = default);
    }
}
