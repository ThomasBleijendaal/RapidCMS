using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEntityVariantSetup ResolveSetup()
        {
            throw new NotImplementedException();
        }

        public IEntityVariantSetup ResolveSetup(string alias)
        {
            if (!_types.TryGetValue(alias, out var type))
            {
                throw new InvalidOperationException($"Cannot find type with alias {alias}.");
            }

            return new EntityVariantSetup(alias, default, type, alias);
        }
    }
}
