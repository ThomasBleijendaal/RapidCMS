using System;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Forms
{
    internal class PropertyState : FormState
    {
        public PropertyState(IPropertyMetadata property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public bool WasValidated { get; set; }
        public bool IsModified { get; set; }
        public bool IsBusy { get; set; }

        public IPropertyMetadata Property { get; }
    }
}
