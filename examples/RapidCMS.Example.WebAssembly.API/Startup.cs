using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RapidCMS.Example.Shared.AuthorizationHandlers;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
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
            services.AddScoped<JsonRepository<ConventionalPerson>>();
            services.AddScoped<JsonRepository<Country>>();
            services.AddScoped<JsonRepository<User>>();
            services.AddScoped<JsonRepository<TagGroup>>();
            services.AddScoped<JsonRepository<Tag>>();
            services.AddScoped<JsonRepository<EntityVariantBase>>();
            services.AddSingleton<MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();
            services.AddSingleton<IConverter<MappedEntity, DatabaseEntity>, Mapper>();
            services.AddSingleton<DatabaseEntityDataViewBuilder>();

            services.AddTransient<Base64TextFileUploadHandler>();
            services.AddTransient<Base64ImageUploadHandler>();

            if (ConfigureAuthentication)
            {
                ConfigureADAuthentication(services);
                services.AddSingleton<IAuthorizationHandler, VeryPermissiveAuthorizationHandler>();
            }

            services.AddRapidCMSApi(config =>
            {
                if (!ConfigureAuthentication)
                {
                    config.AllowAnonymousUser();
                }

                // then register the repositories to RapidCMS API, so it can create controllers for them
                config.RegisterRepository<Person, JsonRepository<Person>>();
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
            });

            services.AddCors();
            services
                .AddControllers(config =>
                {
                    // to allow for automatic setup of the repository controllers, the route convention must be added here
                    config.Conventions.AddRapidCMSRouteConvention();

                    if (ConfigureAuthentication)
                    {
                        config.Filters.Add(new AuthorizeFilter("default"));
                    }
                })
                .AddNewtonsoftJson()
                .ConfigureApplicationPartManager(configure =>
                {
                    // and for each route convention a controller should be added the feature provider
                    configure.FeatureProviders.AddRapidCMSControllerFeatureProvider();
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

        private void ConfigureADAuthentication(IServiceCollection services)
        {
            services
                .AddAuthorization(o =>
                {
                    o.AddPolicy("default", builder =>
                    {
                        builder.RequireAuthenticatedUser();
                    });
                });

            services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Authority = Configuration["AzureAd:Instance"];
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Both App ID URI and client id are valid audiences in the access token
                        ValidAudiences = new List<string>
                        {
                            Configuration["AzureAd:ClientId"]
                        }
                    };
                });
        }
    }
}
