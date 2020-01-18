using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Services.Messages
{
    internal class MessageService : IMessageService
    {
        private readonly List<Message> _messages = new List<Message>();

        public IEnumerable<Message> Messages => _messages;

        public event EventHandler? OnNewMessage;

        public void AddMessage(MessageType type, string content)
        {
            _messages.Add(new Message(type, content));
            OnNewMessage?.Invoke(this, new EventArgs());
        }

        public void ClearMessages()
        {
            _messages.Clear();
            OnNewMessage?.Invoke(this, new EventArgs());
        }
    }
}
