using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Models.Setup
{
    internal class ElementSetup : IElementSetup
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
