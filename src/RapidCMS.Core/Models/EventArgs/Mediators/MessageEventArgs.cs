using System;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.EventArgs.Mediators;

public class MessageEventArgs : IMediatorEventArgs
{
    public MessageEventArgs(MessageType type, string content)
    {
        Type = type;
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }

    public MessageType Type { get; set; }
    public string Content { get; set; }
}
