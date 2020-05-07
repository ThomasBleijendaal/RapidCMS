using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Resolvers.Setup;

namespace RapidCMS.Core.Extensions
{
    internal static class SetupResolverExtension
    {
        public static IEnumerable<TSetup> ResolveSetup<TSetup, TConfig>(this ISetupResolver<TSetup, TConfig> resolver, IEnumerable<TConfig> configs)
        {
            return configs.Select(config => resolver.ResolveSetup(config));
        }
    }
}
