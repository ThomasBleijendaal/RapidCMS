using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Extensions
{
    internal static class SetupResolverExtension
    {
        public static IEnumerable<TSetup> ResolveSetup<TSetup, TConfig>(this ISetupResolver<TSetup, TConfig> resolver, IEnumerable<TConfig> configs, ICollectionSetup? collection = default)
        {
            return configs.Select(config => resolver.ResolveSetup(config, collection));
        }
    }
}
