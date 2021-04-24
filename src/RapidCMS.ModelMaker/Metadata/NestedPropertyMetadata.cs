using System;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.ModelMaker.Metadata
{
    internal class NestedPropertyMetadata<TParentEntity, TEntity, TProperty> : IFullPropertyMetadata
    {
        public NestedPropertyMetadata(
            Func<TParentEntity, TEntity> parent,
            IFullPropertyMetadata propertyMetadata)
        {
            PropertyType = propertyMetadata.PropertyType;
            PropertyName = propertyMetadata.PropertyName;
            Fingerprint = $"{propertyMetadata.Fingerprint}::{typeof(TParentEntity).Name}";

            Getter = x =>
            {
                if (parent.Invoke((TParentEntity)x) is object obj)
                {
                    return propertyMetadata.Getter.Invoke(obj);
                }

                return default!;
            };
            Setter = (x, y) =>
            {
                if (parent.Invoke((TParentEntity)x) is object obj)
                {
                    propertyMetadata.Setter.Invoke(obj, y);
                }
            };

            ObjectType = typeof(TParentEntity);
            OriginalExpression = propertyMetadata.OriginalExpression;
        }

        public Type PropertyType { get; }

        public string PropertyName { get; }
        public Type ObjectType { get; }

        public Func<object, object> Getter { get; }
        public Action<object, object> Setter { get; }

        public string Fingerprint { get; }

        public LambdaExpression OriginalExpression { get; }
    }
}
