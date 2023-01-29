using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Core.Mediators
{
    public class MediatorSerivceRequestHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public MediatorSerivceRequestHandler(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> HandleRequestAsync<TResponse>(IRequest<TResponse> serviceRequest)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(serviceRequest.GetType(), typeof(TResponse));

            if (_serviceProvider.GetRequiredService(handlerType) is not { } handler)
            {
                throw new InvalidOperationException($"Cannot find IRequestHandler<T,T> for {serviceRequest.GetType()} and {typeof(TResponse)}");
            }

            return await ((IRequestHandler<IRequest<TResponse>, TResponse>)handler).HandleAsync(serviceRequest);
        }
    }
}
