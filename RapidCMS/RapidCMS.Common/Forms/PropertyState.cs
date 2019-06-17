using System;
using System.Collections.Generic;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Forms
{
    internal class PropertyState
    {
        private List<string> _messages = new List<string>();

        private IPropertyMetadata _property;

        public PropertyState(IPropertyMetadata property)
        {
            _property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public bool WasValidated { get; set; }
        public bool IsModified { get; set; }
        public bool IsBusy { get; set; }

        public IEnumerable<string> GetValidationMessages()
        {
            return _messages;
        }

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }
    }
}
