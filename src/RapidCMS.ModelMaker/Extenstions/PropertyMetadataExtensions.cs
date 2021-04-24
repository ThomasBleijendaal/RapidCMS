using System;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.ModelMaker.Metadata;

namespace RapidCMS.ModelMaker.Extenstions
{
    public static class PropertyMetadataExtensions
    {
        public static IFullPropertyMetadata Nest<TParentEntity, TEntity>(
            this IFullPropertyMetadata propertyMetadata,
            Func<TParentEntity, TEntity?> parentGettter)
        {
            var propertyMetadataType = typeof(NestedPropertyMetadata<,,>)
                .MakeGenericType(typeof(TParentEntity), typeof(TEntity), propertyMetadata.PropertyType);

            return Activator.CreateInstance(propertyMetadataType, parentGettter, propertyMetadata) as IFullPropertyMetadata
                ?? throw new InvalidOperationException();
        }
    }
}
