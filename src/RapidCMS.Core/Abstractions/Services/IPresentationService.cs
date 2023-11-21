using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Services;

public interface IPresentationService
{
    Task<TResult> GetEntityAsync<TRequest, TResult>(TRequest request) where TResult : class;
    Task<TResult> GetEntitiesAsync<TRequest, TResult>(TRequest request) where TResult : class;

    Task<IEnumerable<TypeRegistrationSetup>> GetPageAsync(string pageAlias);
}
