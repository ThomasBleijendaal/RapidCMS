using System;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class EntityVariantConfigExtensions
    {
        public static EntityVariant ToEntityVariant(this EntityVariantConfig variant)
        {
            return new EntityVariant
            {
                Alias = variant.Type.FullName?.ToUrlFriendlyString() ?? throw new InvalidOperationException("The Type of an EntityVariant should have a FullName"),
                Icon = variant.Icon,
                Name = variant.Name,
                Type = variant.Type
            };
        }
    }
}
