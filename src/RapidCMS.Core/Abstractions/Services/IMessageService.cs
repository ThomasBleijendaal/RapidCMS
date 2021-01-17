using System;
using System.Collections.Generic;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Abstractions.Services
{
    [Obsolete("Replace with IMediator")]
    public interface IMessageService
    {
        void AddMessage(MessageType type, string content);
        IEnumerable<Message> Messages { get; }
        void ClearMessages();
        event EventHandler OnNewMessage;
    }
}
