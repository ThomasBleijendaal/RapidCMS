using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Example.Shared.AuthorizationHandlers;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Example.Shared.Validators;
using RapidCMS.Repositories;

namespace RapidCMS.Example.WebAssembly.API
{
    public class Startup
    {
        private const bool ConfigureAuthentication = false;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // first, add the repositories to the DI
            services.AddScoped<JsonRepository<Person>>();
            services.AddScoped<JsonRepository<Details>>();
            services.AddScoped<JsonRepository<ConventionalPerson>>();
            services.AddScoped<JsonRepository<Country>>();
            services.AddScoped<JsonRepository<User>>();
            services.AddScoped<JsonRepository<TagGroup>>();
            services.AddScoped<JsonRepository<Tag>>();
            services.AddScoped<JsonRepository<EntityVariantBase>>();
            services.AddScoped<MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();
            services.AddSingleton<IConverter<MappedEntity, DatabaseEntity>, Mapper>();
            services.AddSingleton<DatabaseEntityDataViewBuilder>();

            services.AddTransient<Base64TextFileUploadHandler>();
            services.AddTransient<Base64ImageUploadHandler>();

            // TODO: the country entity is validated by a FluentValidator
            services.AddSingleton<CountryValidator>();

            if (ConfigureAuthentication)
            {
                ConfigureOpenIDConnectAuthentication(services);
                services.AddSingleton<IAuthorizationHandler, VeryPermissiveAuthorizationHandler>();
            }

            services.AddRapidCMSWebApi(config =>
            {
                if (!ConfigureAuthentication)
                {
                    // it is highly recommended that you add authentication to your API, but for starting
                    // quickly the AllowAnonymousUser is also supported in the API. 
                    config.AllowAnonymousUser();
                }

                // then register the repositories to RapidCMS API, so it can create controllers for them
                config.RegisterRepository<Person, JsonRepository<Person>>();
                config.RegisterRepository<Details, JsonRepository<Details>>();
                config.RegisterRepository<ConventionalPerson, JsonRepository<ConventionalPerson>>();
                config.RegisterRepository<Country, JsonRepository<Country>>();
                config.RegisterRepository<TagGroup, JsonRepository<TagGroup>>();
                config.RegisterRepository<Tag, JsonRepository<Tag>>();
                config.RegisterRepository<EntityVariantBase, JsonRepository<EntityVariantBase>>();
                config.RegisterRepository<MappedEntity, DatabaseEntity, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();

                // if collections use data view builders, they can also be backed by an API
                // if a collection uses a ApiRepository, then it's data view builder must also be API backed
                config.RegisterDataViewBuilder<DatabaseEntityDataViewBuilder>("mapped");

                config.RegisterFileUploadHandler<Base64TextFileUploadHandler>();
                config.RegisterFileUploadHandler<Base64ImageUploadHandler>();

                config.RegisterEntityValidator<Country, CountryValidator>(new CountryValidator.Config
                {
                    ForbiddenContinentName = "fdsafdsa",
                    ForbiddenCountryName = "fdsa"
                });
            });

            services.AddCors();
            // this lines the generic repository controllers up for use by your mvc application, but still gives
            // you access to the IMvcBuilder and MvcOptions for extra configuration like authentication
            services.AddRapidCMSControllers(config =>
            {
                if (ConfigureAuthentication)
                {
                    config.Filters.Add(new AuthorizeFilter("default"));
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors(builder => builder
                .WithOrigins("https://localhost:5001")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureOpenIDConnectAuthentication(IServiceCollection services)
        {
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("default", builder =>
                    {
                        builder.RequireAuthenticatedUser();
                    });
                });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    Configuration.Bind("AzureAd", options);
                });
        }
    }
}
