using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Models.Setup
{
    public class ElementSetup
    {
        public ElementSetup(IPropertyMetadata id, IReadOnlyList<IExpressionMetadata> labels)
        {
            IdProperty = id ?? throw new ArgumentNullException(nameof(id));
            DisplayProperties = labels ?? throw new ArgumentNullException(nameof(labels));
        }

        public IPropertyMetadata IdProperty { get; set; }
        public IReadOnlyList<IExpressionMetadata> DisplayProperties { get; set; }
    }
}
