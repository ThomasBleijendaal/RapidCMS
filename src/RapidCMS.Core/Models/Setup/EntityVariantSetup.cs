using System;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    public class EntityVariantSetup
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

            Alias = variant.Type.FullName?.ToUrlFriendlyString() ?? throw new InvalidOperationException("The Type of an EntityVariant should have a FullName");
            Icon = variant.Icon;
            Name = variant.Name;
            Type = variant.Type;
        }

        public string Name { get; internal set; }
        public string? Icon { get; internal set; }
        public Type Type { get; internal set; }
        public string Alias { get; internal set; }
    }
}
