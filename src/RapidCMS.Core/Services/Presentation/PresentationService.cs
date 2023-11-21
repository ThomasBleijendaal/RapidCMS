using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Services.Presentation;

internal class PresentationService : IPresentationService
{
    private readonly IEnumerable<IPresentationDispatcher> _dispatchers;

    public PresentationService(IEnumerable<IPresentationDispatcher> dispatchers)
    {
        _dispatchers = dispatchers;
    }

    public Task<TResult> GetEntitiesAsync<TRequest, TResult>(TRequest request) where TResult : class
        => _dispatchers.GetTypeFromList<IPresentationDispatcher<TRequest, TResult>>()?.GetAsync(request)
            ?? throw new InvalidOperationException();

    public Task<TResult> GetEntityAsync<TRequest, TResult>(TRequest request) where TResult : class
        => _dispatchers.GetTypeFromList<IPresentationDispatcher<TRequest, TResult>>()?.GetAsync(request)
            ?? throw new InvalidOperationException();

    public Task<IEnumerable<TypeRegistrationSetup>> GetPageAsync(string pageAlias)
        => _dispatchers.GetTypeFromList<IPresentationDispatcher<string, IEnumerable<TypeRegistrationSetup>>>()?.GetAsync(pageAlias)
        ?? throw new InvalidOperationException();
}
