using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Forms
{
    internal class PropertyState
    {
        private readonly List<string> _messages = new List<string>();

        public PropertyState(IPropertyMetadata property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public bool WasValidated { get; set; }
        public bool IsModified { get; set; }
        public bool IsBusy { get; set; }

        public IPropertyMetadata Property { get; }

        public IEnumerable<string> GetValidationMessages()
        {
            foreach (var message in _messages)
            {
                yield return message;
            }
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }
    }
}
