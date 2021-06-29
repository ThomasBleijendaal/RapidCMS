using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace RapidCMS.Api.Functions
{
    // temporary fix. see https://github.com/Azure/azure-functions-dotnet-worker/issues/522
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
