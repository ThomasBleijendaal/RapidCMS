using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Extensions
{
    internal static class SetupResolverExtension
    {
        public static IResolvedSetup<IEnumerable<TSetup>> ResolveSetup<TSetup, TConfig>(this ISetupResolver<TSetup, TConfig> resolver, IEnumerable<TConfig> configs, ICollectionSetup? collection = default)
            where TConfig : notnull
        {
            var allCachable = true;
            return new ResolvedSetup<IEnumerable<TSetup>>(configs.Select(config => resolver.ResolveSetup(config, collection).CheckIfCachable(ref allCachable)), allCachable);
        }

        public static TSetup CheckIfCachable<TSetup>(this IResolvedSetup<TSetup> resolvedSetup, ref bool cachable)
        {
            cachable = cachable && resolvedSetup.Cachable;

            return resolvedSetup.Setup;
        }
    }
}
