using System;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Dispatchers
{
    [Obsolete]
    internal interface IInteractionDispatcher
    {
    }

    [Obsolete]
    internal interface IInteractionDispatcher<TRequest, TResponse> : IInteractionDispatcher
    {
        Task<TResponse> InvokeAsync(TRequest request);
    }
}
