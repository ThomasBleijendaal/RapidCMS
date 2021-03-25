using System;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerEntityExpressionMetadata : IExpressionMetadata
    {
        public ModelMakerEntityExpressionMetadata(string name, Func<ModelMakerEntity, string?> getter)
        {
            PropertyName = name;
            StringGetter = x => getter.Invoke((ModelMakerEntity)x) ?? string.Empty;
        }

        public string PropertyName { get; }

        public Func<object, string> StringGetter { get; }
    }


}
