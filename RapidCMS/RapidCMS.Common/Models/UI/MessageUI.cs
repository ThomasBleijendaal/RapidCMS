using System;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.UI
{
    public class MessageUI
    {
        public MessageUI(MessageType type, string message)
        {
            Type = type;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public MessageType Type { get; set; }
        public string Message { get; set; }
    }
}
