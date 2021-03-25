using System;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerEntityPropertyMetadata : IFullPropertyMetadata
    {
        public ModelMakerEntityPropertyMetadata(
            Type propertyType,
            string propertyName,
            Func<ModelMakerEntity, object?> getter,
            Action<ModelMakerEntity, object?> setter,
            string fingerprint)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            Fingerprint = fingerprint;

            Getter = x => getter.Invoke((ModelMakerEntity)x);
            Setter = (x, y) => setter.Invoke((ModelMakerEntity)x, y);

            ObjectType = typeof(ModelMakerEntity);
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
