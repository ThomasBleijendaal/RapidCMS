using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Dispatchers;

internal interface IPresentationDispatcher
{

}

internal interface IPresentationDispatcher<TRequest, TResponse> : IPresentationDispatcher
    where TResponse : class
{
    Task<TResponse> GetAsync(TRequest request);
}
