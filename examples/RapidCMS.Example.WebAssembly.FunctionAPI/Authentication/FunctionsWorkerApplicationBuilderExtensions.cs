﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace RapidCMS.Example.WebAssembly.FunctionAPI.Authentication;

public static class FunctionsWorkerApplicationBuilderExtensions
{
    // PREVIEW: this class is temporary
    public static IFunctionsWorkerApplicationBuilder UseAuthorization(this IFunctionsWorkerApplicationBuilder builder)
    {
        builder.Services.AddSingleton<AuthenticationMiddleware>();

        return builder.Use(next =>
        {
            return context =>
            {
                var middleware = context.InstanceServices.GetRequiredService<AuthenticationMiddleware>();
                return middleware.InvokeAsync(context, next);
            };
        });
    }
}
