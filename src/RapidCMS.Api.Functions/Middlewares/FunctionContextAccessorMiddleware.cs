using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using RapidCMS.Api.Functions.Abstractions;

namespace RapidCMS.Api.Functions.Middlewares
{
    public class FunctionContextAccessorMiddleware : IFunctionsWorkerMiddleware
    {
        private IFunctionContextAccessor FunctionContextAccessor { get; }

        public FunctionContextAccessorMiddleware(IFunctionContextAccessor accessor)
        {
            FunctionContextAccessor = accessor;
        }

        public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (FunctionContextAccessor.FunctionContext != null)
            {
                // This should never happen because the context should be localized to the current Task chain.
                // But if it does happen (perhaps the implementation is bugged), then we need to know immediately so it can be fixed.
                throw new InvalidOperationException($"Unable to initalize {nameof(IFunctionContextAccessor)}: context has already been initialized.");
            }

            FunctionContextAccessor.FunctionContext = context;

            return next(context);
        }
    }
}
