using System.Threading.Tasks;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IInteractionService
    {
        Task<TResponse> InteractAsync<TRequest, TResponse>(TRequest request, ViewState state);
    }
}
