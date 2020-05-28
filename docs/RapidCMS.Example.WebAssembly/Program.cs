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
        private static Uri _baseUri = new Uri("https://localhost:5003");

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddAuthorizationCore();

            // with LocalStorageRepository collections can store their data in the local storage of
            // the user, making personalisation quite easy
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<BaseRepository<Person>, LocalStorageRepository<Person>>();

            builder.Services.AddScoped<BaseRepository<ConventionalPerson>, ApiRepository<ConventionalPerson>>();
            builder.Services.AddRapidCMSApiHttpClient<ConventionalPerson>(_baseUri, "person-convention");
            builder.Services.AddScoped<BaseRepository<Country>, ApiRepository<Country>>();
            builder.Services.AddRapidCMSApiHttpClient<Country>(_baseUri, "country");
            builder.Services.AddScoped<BaseRepository<User>, ApiRepository<User>>();
            builder.Services.AddRapidCMSApiHttpClient<User>(_baseUri, "user");
            builder.Services.AddScoped<BaseRepository<TagGroup>, ApiRepository<TagGroup>>();
            builder.Services.AddRapidCMSApiHttpClient<TagGroup>(_baseUri, "taggroup");
            builder.Services.AddScoped<BaseRepository<Tag>, ApiRepository<Tag>>();
            builder.Services.AddRapidCMSApiHttpClient<Tag>(_baseUri, "tag");

            builder.Services.AddSingleton<BaseMappedRepository<MappedEntity, DatabaseEntity>, ApiMappedRepository<MappedEntity, DatabaseEntity>>();
            builder.Services.AddRapidCMSApiHttpClient<MappedEntity, DatabaseEntity>(_baseUri, "mapped");
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
