using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Components;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Example.Shared.Validators;
using RapidCMS.Example.WebAssembly.Components;
using RapidCMS.Repositories;
using RapidCMS.Repositories.ApiBridge;

namespace RapidCMS.Example.WebAssembly
{
    public class Program
    {
        private const bool ConfigureAuthentication = false;
        // web api 
        private static readonly Uri BaseUri = new Uri("https://localhost:5003/api/");
        // function api
        //private static readonly Uri BaseUri = new Uri("http://localhost:7074/api/");

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAuthorizationCore();

            if (ConfigureAuthentication)
            {
                // the builder given to AddRapidCMSApiTokenAuthorization is used to build all message handlers
                // for each of the repositories http client.
                builder.Services.AddRapidCMSApiTokenAuthorization(sp =>
                {
                    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
                    handler.ConfigureHandler(new[] { BaseUri.AbsoluteUri });
                    return handler;
                });
            }

            // with LocalStorageRepository collections can store their data in the local storage of
            // the user, making personalisation quite easy
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<BaseRepository<User>, LocalStorageRepository<User>>();

            // it's not required to add your repositories under the base repository
            // but this allows the Server and the WebAssembly examples to share the collection configuration

            // AddRapidCMSApiRepository allows you to add ApiRepositories which are lined up with a correct HttpClient to 
            // work seamlessly with a repository on the API side of things (See RapidCMS.Example.WebAssembly.API)
            if (!ConfigureAuthentication)
            {
                builder.Services.AddRapidCMSApiRepository<BaseRepository<Person>, ApiRepository<Person>>(BaseUri);
                builder.Services.AddRapidCMSApiRepository<BaseRepository<Details>, ApiRepository<Details>>(BaseUri);
                builder.Services.AddRapidCMSApiRepository<BaseRepository<ConventionalPerson>, ApiRepository<ConventionalPerson>>(BaseUri);
                builder.Services.AddRapidCMSApiRepository<BaseRepository<Country>, ApiRepository<Country>>(BaseUri);
                builder.Services.AddRapidCMSApiRepository<BaseRepository<TagGroup>, ApiRepository<TagGroup>>(BaseUri);
                builder.Services.AddRapidCMSApiRepository<BaseRepository<Tag>, ApiRepository<Tag>>(BaseUri);
                builder.Services.AddRapidCMSApiRepository<BaseRepository<EntityVariantBase>, ApiRepository<EntityVariantBase>>(BaseUri);

                // api repositories can also be mapped
                builder.Services.AddRapidCMSApiRepository<
                    BaseMappedRepository<MappedEntity, DatabaseEntity>,
                    ApiMappedRepository<MappedEntity, DatabaseEntity>>(BaseUri);
            }
            else
            {
                // The AuthorizationMessageHandler forwards the auth token from the frontend to the backend, allowing you to validate the user easily
                builder.Services.AddRapidCMSAuthenticatedApiRepository<BaseRepository<Person>, ApiRepository<Person>, AuthorizationMessageHandler>(BaseUri);
                builder.Services.AddRapidCMSAuthenticatedApiRepository<BaseRepository<Details>, ApiRepository<Details>, AuthorizationMessageHandler>(BaseUri);
                builder.Services.AddRapidCMSAuthenticatedApiRepository<BaseRepository<ConventionalPerson>, ApiRepository<ConventionalPerson>, AuthorizationMessageHandler>(BaseUri);
                builder.Services.AddRapidCMSAuthenticatedApiRepository<BaseRepository<Country>, ApiRepository<Country>, AuthorizationMessageHandler>(BaseUri);
                builder.Services.AddRapidCMSAuthenticatedApiRepository<BaseRepository<TagGroup>, ApiRepository<TagGroup>, AuthorizationMessageHandler>(BaseUri);
                builder.Services.AddRapidCMSAuthenticatedApiRepository<BaseRepository<Tag>, ApiRepository<Tag>, AuthorizationMessageHandler>(BaseUri);
                builder.Services.AddRapidCMSAuthenticatedApiRepository<BaseRepository<EntityVariantBase>, ApiRepository<EntityVariantBase>, AuthorizationMessageHandler>(BaseUri);

                // api repositories can also be mapped
                builder.Services.AddRapidCMSAuthenticatedApiRepository<
                    BaseMappedRepository<MappedEntity, DatabaseEntity>,
                    ApiMappedRepository<MappedEntity, DatabaseEntity>,
                    AuthorizationMessageHandler>(BaseUri);
                builder.Services.AddSingleton<DatabaseEntityDataViewBuilder>();
            }

            builder.Services.AddSingleton<DatabaseEntityDataViewBuilder>();
            builder.Services.AddScoped<BaseRepository<Counter>, CounterRepository>();

            builder.Services.AddSingleton<RandomNameActionHandler>();
            builder.Services.AddSingleton<NavigateToPersonHandler>();

            builder.Services.AddTransient<ITextUploadHandler, Base64ApiTextUploadHandler>();
            builder.Services.AddTransient<IImageUploadHandler, Base64ApiImageUploadHandler>();

            // the country entity is validated by a FluentValidator
            builder.Services.AddSingleton<CountryValidator>();

            if (!ConfigureAuthentication)
            {
                // just like the repositories, the file uploads are also forwarded to the backend API
                // the handler should also be registered on the API side
                builder.Services.AddRapidCMSFileUploadApiHttpClient<Base64TextFileUploadHandler>(BaseUri);
                builder.Services.AddRapidCMSFileUploadApiHttpClient<Base64ImageUploadHandler>(BaseUri);
            }
            else
            {
                // when authentication is enabled, the auth token from the frontend should be forwarded to the file upload API
                builder.Services.AddRapidCMSAuthenticatedFileUploadApiHttpClient<Base64TextFileUploadHandler, AuthorizationMessageHandler>(BaseUri);
                builder.Services.AddRapidCMSAuthenticatedFileUploadApiHttpClient<Base64ImageUploadHandler, AuthorizationMessageHandler>(BaseUri);
            }
            
            if (ConfigureAuthentication)
            {
                ConfigureOpenIDConnectAuthentication(builder);
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

                // configure 5 as number of concurrent HTTP requests to the backend API
                config.Advanced.SemaphoreCount = 5;

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

        private static void ConfigureOpenIDConnectAuthentication(WebAssemblyHostBuilder builder)
        {
            // For OIDC
            builder.Services.AddOidcAuthentication(config =>
            {
                builder.Configuration.Bind("OnlineDevOIDC", config);
            });

            // For AD
            //builder.Services.AddMsalAuthentication(options =>
            //{
            //    builder.Configuration.Bind("AzureAd", options.ProviderOptions);
            //});
        }
    }
}
