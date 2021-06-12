using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IElementSetup
    {
        IPropertyMetadata IdProperty { get; set; }

        IReadOnlyList<IExpressionMetadata> DisplayProperties { get; set; }
    }
}
