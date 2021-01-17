using System;

namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IMediatorEventConverter : IDisposable
    {
        void RegisterConversion(IMediator mediator);
    }
}
