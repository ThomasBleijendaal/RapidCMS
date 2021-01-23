using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using RapidCMS.Repositories;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.WebAssembly.FunctionAPI
{
    class Program
    {
        private const bool ConfigureAuthentication = false;

        static async Task Main(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif

            var host = new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddCommandLine(args);
                })
                .ConfigureFunctionsWorker((context, builder) =>
                {
                    builder.UseFunctionExecutionMiddleware();
                })
                .ConfigureServices(services =>
                {
                    //services.AddScoped<JsonRepository<Person>>();

                    //services.AddRapidCMSFunctions(config =>
                    //{
                    //    if (!ConfigureAuthentication)
                    //    {
                    //        config.AllowAnonymousUser();
                    //    }

                    //    config.RegisterRepository<Person, JsonRepository<Person>>();
                    //});
                })
                .Build();

            await host.RunAsync();
        }
    }
}
