using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IInteractionService
    {
        Task<TResponse> InteractAsync<TRequest, TResponse>(TRequest request);
    }
}
