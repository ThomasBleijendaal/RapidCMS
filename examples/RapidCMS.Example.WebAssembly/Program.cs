using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Components;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Example.WebAssembly.Components;
using RapidCMS.Repositories;
using RapidCMS.Repositories.ApiBridge;

namespace RapidCMS.Example.WebAssembly
{
    public class Program
    {
        private const bool ConfigureAuthentication = false;
        private static readonly Uri BaseUri = new Uri("https://localhost:5003");

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAuthorizationCore();

            if (ConfigureAuthentication)
            {
                builder.Services.AddRapidCMSApiTokenAuthorization(() => BaseUri.AbsoluteUri);
            }

            // it's not required to add your repositories under the base repository
            // but this allows the Server and the WebAssembly examples to share the collection configuration
            // futhtermore, the AddRapidCMSApiRepository allows you to add ApiRepositories which are lined up with a correct HttpClient to 
            // work seamlessly with a repository on the API side of things (See RapidCMS.Example.WebAssembly.API)
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Person>, ApiRepository<Person, JsonRepository<Person>>>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Details>, ApiRepository<Details, JsonRepository<Details>>>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());
            builder.Services.AddRapidCMSApiRepository<BaseRepository<ConventionalPerson>, ApiRepository<ConventionalPerson, JsonRepository<ConventionalPerson>>>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Country>, ApiRepository<Country, JsonRepository<Country>>>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());
            builder.Services.AddRapidCMSApiRepository<BaseRepository<TagGroup>, ApiRepository<TagGroup, JsonRepository<TagGroup>>>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Tag>, ApiRepository<Tag, JsonRepository<Tag>>>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());

            // with LocalStorageRepository collections can store their data in the local storage of
            // the user, making personalisation quite easy
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<BaseRepository<User>, LocalStorageRepository<User>>();

            // entity variants are not supported in Api Contexts, so this repository is added as a local storage repository
            builder.Services.AddScoped<BaseRepository<EntityVariantBase>, LocalStorageRepository<EntityVariantBase>>();

            // api repositories can also be mapped
            builder.Services.AddRapidCMSApiRepository<
                BaseMappedRepository<MappedEntity, DatabaseEntity>,
                ApiMappedRepository<MappedEntity, DatabaseEntity, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());
            builder.Services.AddSingleton<DatabaseEntityDataViewBuilder>();

            builder.Services.AddScoped<BaseRepository<Counter>, CounterRepository>();

            builder.Services.AddSingleton<RandomNameActionHandler>();
            builder.Services.AddSingleton<NavigateToPersonHandler>();

            builder.Services.AddTransient<ITextUploadHandler, Base64ApiTextUploadHandler>();
            builder.Services.AddRapidCMSFileUploadApiHttpClient<Base64TextFileUploadHandler>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());
            builder.Services.AddTransient<IImageUploadHandler, Base64ApiImageUploadHandler>();
            builder.Services.AddRapidCMSFileUploadApiHttpClient<Base64ImageUploadHandler>(BaseUri)
                .If(ConfigureAuthentication, httpClient => httpClient.AddHttpMessageHandler<TokenAuthorizationMessageHandler>());
            
            if (ConfigureAuthentication)
            {
                ConfigureADAuthentication(builder);
            }

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                if (!ConfigureAuthentication)
                {
                    config.AllowAnonymousUser();
                }
                else
                { 
                    config.SetCustomLoginStatus(typeof(LoginStatus));
                    config.SetCustomLoginScreen(typeof(LoginScreen));
                }

                // CRUD editor for simple POCO with recursive sub collections
                // --> see Collections/PersonCollection for the basics of this CMS
                config.AddPersonCollection();

                // CRUD editor with support for one-to-many relation + validation
                // --> see Collections/CountryCollection for one-to-many relation with validation
                config.AddCountryCollection();

                // Custom page with either custom Blazor components, or ListViews or ListEditors of collections
                config.AddPage("beaker", "Some random page", config =>
                {
                    config.AddSection(typeof(CustomSection));
                    config.AddSection("country", edit: false);
                });

                // CRUD editor with validation attributes, custom editor and custom button panes
                // --> see Collections/UserCollection 
                config.AddUserCollection();

                // CRUD editor with nested collection
                // --> see Collections/TagCollection
                config.AddTagCollection();

                // CRUD editor with entity mapping
                config.AddMappedCollection();

                // CRUD editor based on conventions for even more rapid development
                config.AddConventionCollection();

                // CRUD editor with entity variants, so multiple types of entities can be mixed in a single collection
                config.AddEntityVariantCollection();

                // CRUD editor displaying live data, an external process updates the data every second
                config.AddActiveCollection();

                //config.Dashboard.AddSection(typeof(DashboardSection));
                config.Dashboard.AddSection("user", edit: true);
            });

            var host = builder.Build();

            var cmsOptions = host.Services.GetRequiredService<ICms>();
            cmsOptions.IsDevelopment = true;

            await host.RunAsync();
        }

        private static void ConfigureADAuthentication(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("77d6b575-8519-4e3d-9da0-5580bd25b026");
            });
        }
    }
}
