using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Repositories;
using RapidCMS.ModelMaker;
using RapidCMS.Repositories.ApiBridge;

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
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Identity>, ApiRepository<Identity>>(BaseUri);
            builder.Services.AddTransient<IdentityValidator>();

            builder.Services.AddModelMakerCoreCollections();

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.AllowAnonymousUser();
                config.AddIdentityCollection();

                config.Advanced.SemaphoreCount = 5;
            });

            await builder.Build().RunAsync();
        }
    }
}
