using System;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Components;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Example.Shared.Validators;
using RapidCMS.Repositories;
using RapidCMS.Repositories.ApiBridge;

namespace RapidCMS.Example.Maui
{
    public static class MauiProgram
    {
        // web api (run this project along with RapidCMS.Example.WebAssembly.API)
        private static readonly Uri BaseUri = new Uri("https://localhost:5003/api/");
        
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>();

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddAuthorizationCore();


            // with LocalStorageRepository collections can store their data in the local storage of
            // the user, making personalization quite easy
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<BaseRepository<User>, LocalStorageRepository<User>>();

            builder.Services.AddRapidCMSApiRepository<BaseRepository<Person>, ApiRepository<Person>>(BaseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Details>, ApiRepository<Details>>(BaseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<ConventionalPerson>, ApiRepository<ConventionalPerson>>(BaseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Country>, ApiRepository<Country>>(BaseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<TagGroup>, ApiRepository<TagGroup>>(BaseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<Tag>, ApiRepository<Tag>>(BaseUri);
            builder.Services.AddRapidCMSApiRepository<BaseRepository<EntityVariantBase>, ApiRepository<EntityVariantBase>>(BaseUri);

            builder.Services.AddSingleton<DatabaseEntityDataViewBuilder>();
            builder.Services.AddScoped<BaseRepository<Counter>, CounterRepository>();

            builder.Services.AddSingleton<RandomNameActionHandler>();
            builder.Services.AddSingleton<NavigateToPersonHandler>();

            builder.Services.AddTransient<ITextUploadHandler, Base64ApiTextUploadHandler>();
            builder.Services.AddTransient<IImageUploadHandler, Base64ApiImageUploadHandler>();

            // the country entity is validated by a FluentValidator
            builder.Services.AddSingleton<CountryValidator>();

            builder.Services.AddRapidCMSFileUploadApiHttpClient<Base64TextFileUploadHandler>(BaseUri);
            builder.Services.AddRapidCMSFileUploadApiHttpClient<Base64ImageUploadHandler>(BaseUri);

            // api repositories can also be mapped
            builder.Services.AddRapidCMSApiRepository<
                BaseMappedRepository<MappedEntity, DatabaseEntity>,
                ApiMappedRepository<MappedEntity, DatabaseEntity>>(BaseUri);

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.AllowAnonymousUser();

                // configure 5 as number of concurrent HTTP requests to the backend API
                config.Advanced.SemaphoreCount = 5;

                // CRUD editor for simple POCO with recursive sub collections
                // --> see Collections/PersonCollection for the basics of this CMS
                config.AddPersonCollection();

                // CRUD editor with support for one-to-many relation + validation
                // --> see Collections/CountryCollection for one-to-many relation with validation
                config.AddCountryCollection();

                // Custom page with either custom Blazor components, or ListViews or ListEditors of collections
                config.AddPage("TestBeakerSolid", "Green10", "Some random page", config =>
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

            return builder.Build();
        }
    }
}
