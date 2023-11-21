using System;

namespace RapidCMS.Core.Abstractions.Mediators;

public interface IMediatorEventListener : IDisposable
{
    void RegisterListener(IMediator mediator);
}
