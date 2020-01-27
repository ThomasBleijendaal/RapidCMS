using System;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class EntityVariantSetup : IEntityVariantSetup
    {
        public static EntityVariantSetup Undefined = new EntityVariantSetup(default!);

        internal EntityVariantSetup(EntityVariantConfig variant)
        {
            if (variant == default)
            {
                Name = "";
                Type = typeof(object);
                Alias = "";

                return;
            }

            Alias = variant.Type.Name.ToUrlFriendlyString();
            Icon = variant.Icon;
            Name = variant.Name;
            Type = variant.Type;
        }

        internal string Name { get; set; }
        internal string? Icon { get; set; }
        internal Type Type { get; set; }
        internal string Alias { get; set; }

        string IEntityVariantSetup.Name => Name;
        string? IEntityVariantSetup.Icon => Icon;
        Type IEntityVariantSetup.Type => Type;
        string IEntityVariantSetup.Alias => Alias;
    }
}
