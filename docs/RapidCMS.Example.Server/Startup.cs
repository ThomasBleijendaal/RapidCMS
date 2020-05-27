using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Example.Shared.Components;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Repositories;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Example.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // it's not required to add your repositories under the base repository
            // but this allows the Server and the WebAssembly examples to share the collection configuration
            services.AddSingleton<BaseRepository<Person>, JsonRepository<Person>>();
            services.AddSingleton<BaseRepository<ConventionalPerson>, JsonRepository<ConventionalPerson>>();
            services.AddSingleton<BaseRepository<Country>, JsonRepository<Country>>();
            services.AddSingleton<BaseRepository<User>, JsonRepository<User>>();
            services.AddSingleton<BaseRepository<TagGroup>, JsonRepository<TagGroup>>();
            services.AddSingleton<BaseRepository<Tag>, JsonRepository<Tag>>();

            services.AddSingleton<MappedBaseRepository<MappedEntity, DatabaseEntity>, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();
            services.AddSingleton<IConverter<MappedEntity, DatabaseEntity>, Mapper>();
            services.AddSingleton<DatabaseEntityDataViewBuilder>();

            services.AddSingleton<RandomNameActionHandler>();
            services.AddSingleton<Base64TextFileUploadHandler>();
            services.AddSingleton<Base64ImageUploadHandler>();

            services.AddRapidCMSServer(config =>
            {
                config.AllowAnonymousUser();

                config.SetCustomLoginStatus(typeof(LoginStatus));

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

                // the dashboard can be build up of custom Blazor components, or the ListViews or ListEditors of collections
                config.Dashboard.AddSection(typeof(DashboardSection));
                config.Dashboard.AddSection("user", edit: true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRapidCMS(isDevelopment: env.IsDevelopment());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
