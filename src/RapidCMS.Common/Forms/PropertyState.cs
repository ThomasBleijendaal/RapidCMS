using System;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Forms
{
    internal class PropertyState : FormState
    {
        private IPropertyMetadata _property;

        public PropertyState(IPropertyMetadata property)
        {
            _property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public bool WasValidated { get; set; }
        public bool IsModified { get; set; }
        public bool IsBusy { get; set; }
    }
}
