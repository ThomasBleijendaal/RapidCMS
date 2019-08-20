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
                // TODO: is this guid going to work properly?
                Alias = variant.Type.FullName?.ToUrlFriendlyString() ?? Guid.NewGuid().ToString(),
                Icon = variant.Icon,
                Name = variant.Name,
                Type = variant.Type
            };
        }
    }
}
