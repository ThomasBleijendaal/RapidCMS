using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace RapidCMS.Api.Functions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Debugger.Launch();

            var host = new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddCommandLine(args);
                    config.AddEnvironmentVariables();
                })
                .ConfigureFunctionsWorker((context, builder) =>
                {
                    
                }, options => { })
                .ConfigureServices((context, services) =>
                {
                    
                })
                .Build();

            await host.RunAsync();
        }
    }
}
