using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Server.Components;
using RapidCMS.Example.Shared.AuthorizationHandlers;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Components;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Server
{
    public class Startup
    {
        private const bool ConfigureAuthentication = true;

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
            services.AddScoped<BaseRepository<Person>, JsonRepository<Person>>();
            services.AddScoped<BaseRepository<Details>, JsonRepository<Details>>();
            services.AddScoped<BaseRepository<ConventionalPerson>, JsonRepository<ConventionalPerson>>();
            services.AddScoped<BaseRepository<Country>, JsonRepository<Country>>();
            services.AddScoped<BaseRepository<User>, JsonRepository<User>>();
            services.AddScoped<BaseRepository<TagGroup>, JsonRepository<TagGroup>>();
            services.AddScoped<BaseRepository<Tag>, JsonRepository<Tag>>();
            services.AddScoped<BaseRepository<EntityVariantBase>, InMemoryRepository<EntityVariantBase>>();
            services.AddScoped<BaseRepository<Counter>, CounterRepository>();

            services.AddScoped<BaseMappedRepository<MappedEntity, DatabaseEntity>, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();
            services.AddSingleton<IConverter<MappedEntity, DatabaseEntity>, Mapper>();
            services.AddSingleton<DatabaseEntityDataViewBuilder>();

            services.AddSingleton<RandomNameActionHandler>();
            services.AddScoped<NavigateToPersonHandler>();

            // although it's not required to add your own interfaces to the upload handlers, the Server and WebAssembly examples use the
            // same collection configuration, so the DI configuration dictates what handler is used in each case
            services.AddSingleton<ITextUploadHandler, Base64TextFileUploadHandler>();
            services.AddSingleton<IImageUploadHandler, Base64ImageUploadHandler>();

            if (ConfigureAuthentication)
            {
                ConfigureOpenIDConnectAuthentication(services);
                services.AddSingleton<IAuthorizationHandler, VeryPermissiveAuthorizationHandler>();
            }

            services.AddRapidCMSServer(config =>
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void ConfigureOpenIDConnectAuthentication(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "OpenIdConnect";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("OpenIdConnect", options =>
                {
                    Configuration.Bind("OnlineDevOIDC", options);

                    IdentityModelEventSource.ShowPII = true;

                    options.Events.OnSignedOutCallbackRedirect = (ctx) =>
                    {
                        ctx.HandleResponse();
                        ctx.Response.Redirect("/");
                        return Task.CompletedTask;
                    };
                });
        }
    }
}
