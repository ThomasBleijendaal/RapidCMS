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
            var serviceRequestType = serviceRequest.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(serviceRequestType, typeof(TResponse));

            if (_serviceProvider.GetRequiredService(handlerType) is not { } handler)
            {
                throw new InvalidOperationException($"Cannot find IRequestHandler<T,T> for {serviceRequestType} and {typeof(TResponse)}");
            }

            var method = handlerType.GetMethod("HandleAsync");

            var response = (Task<TResponse>)(method?.Invoke(handler, new object?[] { serviceRequest })
                ?? throw new InvalidOperationException("Failed to get Task from IRequestHandler<T,T>"));

            return await response;
        }
    }
}
