using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Services.Persistence
{
    internal class InteractionService : IInteractionService
    {
        private readonly IServiceProvider _serviceProvider;

        public InteractionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResponse> InteractAsync<TRequest, TResponse>(TRequest request, ViewState state)
        {
            var dispatchers = _serviceProvider.GetService<IEnumerable<IInteractionDispatcher>>();

            if (dispatchers.FirstOrDefault(x => x.GetType().GetInterfaces().Any(i => i == typeof(IInteractionDispatcher<TRequest, TResponse>))) is IInteractionDispatcher<TRequest, TResponse> dispatcher)
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
