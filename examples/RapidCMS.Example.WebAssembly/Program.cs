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
            // TODO: add comments like the ServerSide project

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddAuthorizationCore();

            builder.Services.AddRapidCMSApiRepository<BaseRepository<Person>, ApiRepository<Person, JsonRepository<Person>>>(_baseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<ConventionalPerson>, ApiRepository<ConventionalPerson, JsonRepository<ConventionalPerson>>>(_baseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Country>, ApiRepository<Country, JsonRepository<Country>>>(_baseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<TagGroup>, ApiRepository<TagGroup, JsonRepository<TagGroup>>>(_baseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Tag>, ApiRepository<Tag, JsonRepository<Tag>>>(_baseUri);

            // with LocalStorageRepository collections can store their data in the local storage of
            // the user, making personalisation quite easy
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<BaseRepository<User>, LocalStorageRepository<User>>();

            builder.Services.AddRapidCMSApiRepository<
                BaseMappedRepository<MappedEntity, DatabaseEntity>,
                ApiMappedRepository<MappedEntity, DatabaseEntity, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>>(_baseUri);
            builder.Services.AddSingleton<DatabaseEntityDataViewBuilder>();

            builder.Services.AddSingleton<RandomNameActionHandler>();

            builder.Services.AddTransient<ITextUploadHandler, Base64ApiTextUploadHandler>();
            builder.Services.AddRapidCMSFileUploadApiHttpClient<Base64TextFileUploadHandler>(_baseUri);
            builder.Services.AddTransient<IImageUploadHandler, Base64ApiImageUploadHandler>();
            builder.Services.AddRapidCMSFileUploadApiHttpClient<Base64ImageUploadHandler>(_baseUri);

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

            var cmsOptions = host.Services.GetService<ICms>();
            cmsOptions.IsDevelopment = true;

            await host.RunAsync();
        }
    }
}
