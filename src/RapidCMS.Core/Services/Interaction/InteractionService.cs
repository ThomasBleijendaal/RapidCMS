using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Services.Persistence
{
    internal class InteractionService : IInteractionService
    {
        private readonly IEnumerable<IInteractionDispatcher> _dispatchers;

        public InteractionService(IEnumerable<IInteractionDispatcher> dispatchers)
        {
            _dispatchers = dispatchers;
        }

        public Task<TResponse> InteractAsync<TRequest, TResponse>(TRequest request)
            => _dispatchers.GetTypeFromList<IInteractionDispatcher<TRequest, TResponse>>()?.InvokeAsync(request)
            ?? throw new InvalidOperationException("Could not find the correct interaction dispatcher.");
    }
}
