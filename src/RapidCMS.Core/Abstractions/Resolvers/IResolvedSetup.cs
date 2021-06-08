namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface IResolvedSetup<T>
    {
        T Setup { get; }
        bool Cachable { get; }
    }
}
