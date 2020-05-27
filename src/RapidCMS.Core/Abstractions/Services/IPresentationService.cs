using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IPresentationService
    {
        Task<TResult> GetEntityAsync<TRequest, TResult>(TRequest request) where TResult : class;
        Task<TResult> GetEntitiesAsync<TRequest, TResult>(TRequest request) where TResult : class;

        Task<IEnumerable<ITypeRegistration>> GetPageAsync(string pageAlias);
    }
}
