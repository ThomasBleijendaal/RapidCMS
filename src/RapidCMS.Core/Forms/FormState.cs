using System.Collections.Generic;

namespace RapidCMS.Core.Forms
{
    internal class FormState
    {
        private readonly List<string> _messages = new List<string>();

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
