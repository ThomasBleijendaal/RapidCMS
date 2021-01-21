using System;

namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IMediatorRequestEventArgs<TResponse> : IMediatorEventArgs
    {
        Guid RequestId { get; set; }
    }
}
