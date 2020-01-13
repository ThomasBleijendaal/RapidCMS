using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Dispatchers
{
    internal interface IPresenationDispatcher<TRequest, TResponse>
    {
        Task<TResponse> GetAsync(TRequest request);
    }
}
