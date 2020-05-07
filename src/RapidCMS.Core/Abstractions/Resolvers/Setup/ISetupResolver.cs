namespace RapidCMS.Core.Abstractions.Resolvers.Setup
{
    internal interface ISetupResolver<TSetup>
    {
        TSetup ResolveSetup();
        TSetup ResolveSetup(string alias);
    }
}
