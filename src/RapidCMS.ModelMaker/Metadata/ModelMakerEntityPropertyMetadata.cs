using System;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.ModelMaker.Metadata
{
    internal class PropertyMetadata<TEntity, TProperty> : IFullPropertyMetadata
    {
        public PropertyMetadata(
            string propertyName,
            Func<TEntity, TProperty?> getter,
            Action<TEntity, TProperty?> setter,
            string fingerprint)
        {
            PropertyType = typeof(TProperty);
            PropertyName = propertyName;
            Fingerprint = fingerprint;

            Getter = x => getter.Invoke((TEntity)x);
            Setter = (x, y) => setter.Invoke((TEntity)x, (TProperty)y);

            ObjectType = typeof(TEntity);
            OriginalExpression = Expression.Lambda(Expression.Empty());
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
