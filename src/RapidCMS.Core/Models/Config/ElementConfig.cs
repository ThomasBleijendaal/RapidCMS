using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Models.Config
{
    internal class ElementConfig
    {
        public ElementConfig(IPropertyMetadata id, params IExpressionMetadata[] labels)
        {
            IdProperty = id ?? throw new ArgumentNullException(nameof(id));
            DisplayProperties = labels?.ToList() ?? throw new ArgumentNullException(nameof(labels));
        }

        internal IPropertyMetadata IdProperty { get; set; }

        internal IReadOnlyList<IExpressionMetadata> DisplayProperties { get; set; }
    }
}
