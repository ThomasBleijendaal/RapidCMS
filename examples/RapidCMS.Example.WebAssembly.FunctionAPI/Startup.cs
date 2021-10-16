using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Example.Shared.AuthorizationHandlers;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Example.Shared.Validators;
using RapidCMS.Example.WebAssembly.FunctionAPI.Authentication;
using RapidCMS.Repositories;

namespace RapidCMS.Example.WebAssembly.FunctionAPI
{
    public class Startup
    {
        private const bool ConfigureAuthentication = true;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddOptions<AuthenticationConfig>().Bind(Configuration.GetSection("DevOIDC"));

            services.AddAuthorizationCore();
            services.AddSingleton<IAuthorizationHandler, VeryPermissiveAuthorizationHandler>();

            // the country entity is validated by a FluentValidator
            services.AddSingleton<CountryValidator>();

            services.AddRapidCMSFunctions(config =>
            {
                if (!ConfigureAuthentication)
                {
                    config.AllowAnonymousUser();
                }

                config.RegisterRepository<Person, JsonRepository<Person>>();
                config.RegisterRepository<Details, JsonRepository<Details>>();
                config.RegisterRepository<ConventionalPerson, JsonRepository<ConventionalPerson>>();
                config.RegisterRepository<Country, JsonRepository<Country>>();
                config.RegisterRepository<TagGroup, JsonRepository<TagGroup>>();
                config.RegisterRepository<Tag, JsonRepository<Tag>>();
                config.RegisterRepository<EntityVariantBase, JsonRepository<EntityVariantBase>>();
                config.RegisterRepository<MappedEntity, DatabaseEntity, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();

                config.RegisterDataViewBuilder<DatabaseEntityDataViewBuilder>("mapped");

                config.RegisterFileUploadHandler<Base64TextFileUploadHandler>();
                config.RegisterFileUploadHandler<Base64ImageUploadHandler>();
            });
        }

        public void ConfigureWorker(IFunctionsWorkerApplicationBuilder builder)
        {
            builder.UseRapidCMS();

            if (ConfigureAuthentication)
            {
                builder.UseAuthorization();
            }

            builder.UseFunctionExecutionMiddleware();
        }
    }
}
