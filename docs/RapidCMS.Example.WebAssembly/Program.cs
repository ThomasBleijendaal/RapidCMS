using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Components;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Repositories;
using RapidCMS.Repositories.ApiBridge;

namespace RapidCMS.Example.WebAssembly
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            // TODO: automate
            builder.Services.AddHttpClient<ApiRepository<ConventionalPerson>>("person-convention").ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5003/api/_rapidcms/person-convention/"));
            builder.Services.AddHttpClient<ApiRepository<Country>>("country").ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5003/api/_rapidcms/country/"));
            builder.Services.AddHttpClient<ApiRepository<User>>("user").ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5003/api/_rapidcms/user/"));
            builder.Services.AddHttpClient<ApiRepository<TagGroup>>("taggroup").ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5003/api/_rapidcms/taggroup/"));
            builder.Services.AddHttpClient<ApiRepository<Tag>>("tag").ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5003/api/_rapidcms/tag/"));

            //builder.Services.AddScoped(sp => 
            //    new HttpClient { BaseAddress = new Uri("https://localhost:5003/api/_rapidcms/person/") });

            builder.Services.AddAuthorizationCore();

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<BaseRepository<Person>, LocalStorageRepository<Person>>();
            builder.Services.AddScoped<BaseRepository<ConventionalPerson>, ApiRepository<ConventionalPerson>>();
            builder.Services.AddScoped<BaseRepository<Country>, ApiRepository<Country>>();
            builder.Services.AddScoped<BaseRepository<User>, ApiRepository<User>>();
            builder.Services.AddScoped<BaseRepository<TagGroup>, ApiRepository<TagGroup>>();
            builder.Services.AddScoped<BaseRepository<Tag>, ApiRepository<Tag>>();

            builder.Services.AddSingleton<MappedBaseRepository<MappedEntity, DatabaseEntity>, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();
            builder.Services.AddSingleton<IConverter<MappedEntity, DatabaseEntity>, Mapper>();
            builder.Services.AddSingleton<DatabaseEntityDataViewBuilder>();

            builder.Services.AddSingleton<RandomNameActionHandler>();
            builder.Services.AddSingleton<Base64TextFileUploadHandler>();
            builder.Services.AddSingleton<Base64ImageUploadHandler>();

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.AllowAnonymousUser();
                
                config.SetCustomLoginStatus(typeof(LoginStatus));

                config.AddPersonCollection();

                config.AddCountryCollection();

                config.AddPage("beaker", "Some random page", config =>
                {
                    config.AddSection(typeof(CustomSection));
                    config.AddSection("country", edit: false);
                });

                config.AddUserCollection();

                config.AddTagCollection();

                config.AddMappedCollection();

                config.AddConventionCollection();

                config.Dashboard.AddSection(typeof(DashboardSection));
                config.Dashboard.AddSection("user", edit: true);
            });

            var host = builder.Build();

            // TODO
            var cmsOptions = host.Services.GetService<ICms>();
            cmsOptions.IsDevelopment = true;

            await host.RunAsync();
        }
    }
}
