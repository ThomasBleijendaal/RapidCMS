using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.UI
{
    public class MessageUI
    {
        public MessageUI(MessageType type, string message)
        {
            Type = type;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public MessageType Type { get; private set; }
        public string Message { get; private set; }
    }
}
