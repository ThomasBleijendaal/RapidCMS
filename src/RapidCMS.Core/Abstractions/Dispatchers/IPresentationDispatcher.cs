using System;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Dispatchers
{
    [Obsolete]
    internal interface IPresentationDispatcher
    {

    }

    [Obsolete]
    internal interface IPresentationDispatcher<TRequest, TResponse> : IPresentationDispatcher
        where TResponse : class
    {
        Task<TResponse> GetAsync(TRequest request);
    }
}
