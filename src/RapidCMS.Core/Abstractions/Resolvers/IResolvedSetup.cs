namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IResolvedSetup<T>
    {
        T Setup { get; }
        bool Cachable { get; }
    }
}
