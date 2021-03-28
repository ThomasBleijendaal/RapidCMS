using System;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.ModelMaker.Metadata
{
    internal class ExpressionMetadata<TEntity> : IExpressionMetadata
    {
        public ExpressionMetadata(string name, Func<TEntity, string?> getter)
        {
            PropertyName = name;
            StringGetter = x => getter.Invoke((TEntity)x) ?? string.Empty;
        }

        public string PropertyName { get; }

        public Func<object, string> StringGetter { get; }
    }
}
