using System;
using System.Collections.Generic;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Services
{
    public interface IMessageService
    {
        void AddMessage(MessageType type, string message);
        IEnumerable<MessageUI> Messages { get; }
        void ClearMessages();
        event EventHandler OnNewMessage;
    }
}
