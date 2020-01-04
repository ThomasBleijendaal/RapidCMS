using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Data
{
    public class Message
    {
        public Message(MessageType type, string content)
        {
            Type = type;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public MessageType Type { get; set; }
        public string Content { get; set; }
    }
}
