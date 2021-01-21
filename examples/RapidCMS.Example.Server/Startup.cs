using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
        private const bool ConfigureAuthentication = false;

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

            // although it's not required to add your own interfaces to the upload handlers, the Server and WebAssembly examples use the
            // same collection configuration, so the DI configuration dictates what handler is used in each case
            services.AddSingleton<ITextUploadHandler, Base64TextFileUploadHandler>();
            services.AddSingleton<IImageUploadHandler, Base64ImageUploadHandler>();

            if (ConfigureAuthentication)
            {
                ConfigureADAuthentication(services);
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
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void ConfigureADAuthentication(IServiceCollection services)
        {
            // ***********************************************
            // For more info on:
            // Microsoft.AspNetCore.Authentication.AzureAD.UI
            // see:
            // https://bit.ly/2Fv6Zxp
            // This creates a 'virtual' controller 
            // called 'Account' in an Area called 'AzureAd' that allows the
            // 'AzureAd/Account/SignIn' and 'AzureAd/Account/SignOut'
            // links to work
            services
                .AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            // This configures the 'middleware' pipeline
            // This is where code to determine what happens
            // when a person logs in is configured and processed
            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation 
                    // (validating against a single issuer value, as we do in
                    // line of business apps), we inject our own multitenant validation logic
                    ValidateIssuer = false,
                    // If the app is meant to be accessed by entire organizations, 
                    // add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };
                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users 
                        // then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    OnSignedOutCallbackRedirect = context =>
                    {
                        // This is called when a user logs out
                        // redirect them back to the main page
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },
                    // If your application needs to do authenticate single users, 
                    // add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            });
        }
    }
}
