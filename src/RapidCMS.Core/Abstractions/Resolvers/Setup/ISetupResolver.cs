namespace RapidCMS.Core.Abstractions.Resolvers.Setup
{
    internal interface ISetupResolver<TSetup>
    {
        TSetup ResolveSetup();
        TSetup ResolveSetup(string alias);
    }

    internal interface ISetupResolver<TSetup, TConfig>
    {
        TSetup ResolveSetup(TConfig config);
    }
}
