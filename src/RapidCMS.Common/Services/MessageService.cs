using System;
using System.Collections.Generic;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Services
{
    internal class MessageService : IMessageService
    {
        private readonly List<MessageUI> _messages = new List<MessageUI>();

        public IEnumerable<MessageUI> Messages => _messages;

        public event EventHandler? OnNewMessage;

        public void AddMessage(MessageType type, string message)
        {
            _messages.Add(new MessageUI(type, message));
            OnNewMessage?.Invoke(this, new EventArgs());
        }

        public void ClearMessages()
        {
            _messages.Clear();
            OnNewMessage?.Invoke(this, new EventArgs());
        }
    }
}
