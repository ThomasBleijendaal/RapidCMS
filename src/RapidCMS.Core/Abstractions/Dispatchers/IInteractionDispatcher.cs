using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Dispatchers
{
    internal interface IInteractionDispatcher
    {
    }

    internal interface IInteractionDispatcher<TRequest, TResponse>
    {
        Task<TResponse> InvokeAsync(TRequest request);
    }
}
