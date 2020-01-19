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

        // TODO: change to  navigationState ViewContext to allow for more flexibility
        public Task<TResponse> InteractAsync<TRequest, TResponse>(TRequest request, INavigationStateService navigationState)
        {
            if (_interactionDispatchers.FirstOrDefault(x => x.GetType().GetInterfaces().Any(i => i == typeof(IInteractionDispatcher<TRequest, TResponse>))) is IInteractionDispatcher<TRequest, TResponse> dispatcher)
            {
                return dispatcher.InvokeAsync(request, navigationState);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
