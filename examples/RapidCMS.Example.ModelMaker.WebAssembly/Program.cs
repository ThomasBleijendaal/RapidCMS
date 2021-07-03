using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.ModelMaker;

namespace RapidCMS.Example.ModelMaker.WebAssembly
{
    public class Program
    {
        private static readonly Uri BaseUri = new Uri("https://localhost:5003/api/");

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAuthorizationCore();

            builder.Services.AddModelMakerApi(BaseUri);

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.AllowAnonymousUser();

                config.Advanced.SemaphoreCount = 5;

                config.AddModelMakerPlugin();
            });

            await builder.Build().RunAsync();
        }
    }
}
