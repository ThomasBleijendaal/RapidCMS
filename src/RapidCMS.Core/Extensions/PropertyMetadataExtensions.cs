﻿using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Extensions
{
    internal static class PropertyMetadataExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this IPropertyMetadata property)
        {
            return property.ObjectType
                .GetProperties()
                .FirstOrDefault(x => x.Name == property.PropertyName)
                ?.GetCustomAttributes(true)
                .Where(x => x.GetType().IsSameTypeOrDerivedFrom(typeof(T)))
                .Cast<T>()
                ?? Enumerable.Empty<T>();
        }
    }
}
