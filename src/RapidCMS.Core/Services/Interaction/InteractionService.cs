using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;

namespace RapidCMS.Core.Services.Persistence
{
    internal class InteractionService : IInteractionService
    {
        private readonly IEnumerable<IInteractionDispatcher> _interactionDispatchers;

        public InteractionService(IEnumerable<IInteractionDispatcher> interactionDispatchers)
        {
            _interactionDispatchers = interactionDispatchers;
        }

        public Task<TResponse> InteractAsync<TRequest, TResponse>(TRequest request)
        {
            if (_interactionDispatchers.FirstOrDefault(x => x.GetType().GetInterfaces().Any(i => i == typeof(IInteractionDispatcher<TRequest, TResponse>))) is IInteractionDispatcher<TRequest, TResponse> dispatcher)
            {
                return dispatcher.InvokeAsync(request);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
