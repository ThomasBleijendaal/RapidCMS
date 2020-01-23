using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Services.Persistence
{
    internal class InteractionService : IInteractionService
    {
        private readonly IEnumerable<IInteractionDispatcher> _dispatchers;

        public InteractionService(IEnumerable<IInteractionDispatcher> dispatchers)
        {
            _dispatchers = dispatchers;
        }

        public Task<TResponse> InteractAsync<TRequest, TResponse>(TRequest request, ViewState state)
        {
            if (_dispatchers.FirstOrDefault(x => x.GetType().GetInterfaces().Any(i => i == typeof(IInteractionDispatcher<TRequest, TResponse>))) is IInteractionDispatcher<TRequest, TResponse> dispatcher)
            {
                return dispatcher.InvokeAsync(request, state.NavigationState);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
