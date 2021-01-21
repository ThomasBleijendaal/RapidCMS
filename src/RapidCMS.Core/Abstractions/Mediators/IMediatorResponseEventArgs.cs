using System;

namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IMediatorResponseEventArgs<TResponse> : IMediatorEventArgs
    {
        Guid RequestId { get; set; }
        TResponse Response { get; set; }
    }
}
