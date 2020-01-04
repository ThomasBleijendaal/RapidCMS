using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Dispatchers
{
    internal interface IDispatcher<TRequest, TResponse>
    {
        Task<TResponse> InvokeAsync(TRequest request);
    }
}
