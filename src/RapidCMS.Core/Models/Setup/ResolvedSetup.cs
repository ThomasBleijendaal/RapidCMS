using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Core.Models.Setup
{
    public struct ResolvedSetup<T> : IResolvedSetup<T>
    {
        public ResolvedSetup(T setup, bool cachable)
        {
            Setup = setup;
            Cachable = cachable;
        }

        public T Setup { get; private set; }

        public bool Cachable { get; private set; }
    }
}
