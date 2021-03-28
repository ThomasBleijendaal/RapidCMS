using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class GlobalEntityVariantSetupResolver : ISetupResolver<IEntityVariantSetup>
    {
        private readonly IReadOnlyDictionary<string, Type> _types;

        public GlobalEntityVariantSetupResolver()
        {
            _types = typeof(IEntity).GetImplementingTypes().ToDictionary(x => AliasHelper.GetEntityVariantAlias(x));
        }

        public Task<IEntityVariantSetup> ResolveSetupAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEntityVariantSetup> ResolveSetupAsync(string alias)
        {
            if (!_types.TryGetValue(alias, out var type))
            {
                throw new InvalidOperationException($"Cannot find type with alias {alias}.");
            }

            return Task.FromResult<IEntityVariantSetup>(new EntityVariantSetup(alias, default, type, alias));
        }
    }
}
