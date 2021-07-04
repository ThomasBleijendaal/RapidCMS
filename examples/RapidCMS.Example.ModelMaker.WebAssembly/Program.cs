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

            builder.Services.AddModelMakerCoreCollections();

            builder.Services.AddRapidCMSApiRepository<BaseRepository<Blog>, ApiRepository<Blog>>(BaseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Category>, ApiRepository<Category>>(BaseUri);

            builder.Services.AddTransient<BlogValidator>();
            builder.Services.AddTransient<CategoryValidator>();

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.AllowAnonymousUser();

                config.Advanced.SemaphoreCount = 5;

                config.AddBlogCollection();
                config.AddCategoryCollection();
            });

            await builder.Build().RunAsync();
        }
    }
}
