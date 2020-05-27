using System;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class GlobalEntityVariantSetupResolver : ISetupResolver<IEntityVariantSetup>
    {
        public IEntityVariantSetup ResolveSetup()
        {
            throw new NotImplementedException();
        }

        public IEntityVariantSetup ResolveSetup(string alias)
        {
            var type = Type.GetType(alias);
            if (type == null)
            {
                throw new InvalidOperationException($"Cannot find type with alias {alias}.");
            }

            return new EntityVariantSetup(alias, default, type, alias);
        }
    }
}
