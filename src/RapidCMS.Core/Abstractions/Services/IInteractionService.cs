using System;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Services
{
    [Obsolete]
    public interface IInteractionService
    {
        Task<TResponse> InteractAsync<TRequest, TResponse>(TRequest request);
    }
}
