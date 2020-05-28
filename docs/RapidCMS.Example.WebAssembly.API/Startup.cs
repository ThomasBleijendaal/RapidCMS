using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.DataViews;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Repositories;

namespace RapidCMS.Example.WebAssembly.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<JsonRepository<Person>>();
            services.AddScoped<JsonRepository<ConventionalPerson>>();
            services.AddScoped<JsonRepository<Country>>();
            services.AddScoped<JsonRepository<User>>();
            services.AddScoped<JsonRepository<TagGroup>>();
            services.AddScoped<JsonRepository<Tag>>();
            services.AddSingleton<MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();
            services.AddSingleton<IConverter<MappedEntity, DatabaseEntity>, Mapper>();
            services.AddSingleton<DatabaseEntityDataViewBuilder>();

            services.AddTransient<Base64TextFileUploadHandler>();
            services.AddTransient<Base64ImageUploadHandler>();

            services.AddRapidCMSApi(config =>
            {
                // TODO: missing configurations:
                // - OrderBy
                config.RegisterRepository<Person, JsonRepository<Person>>("person");
                config.RegisterRepository<ConventionalPerson, JsonRepository<ConventionalPerson>>("person-convention");
                config.RegisterRepository<Country, JsonRepository<Country>>("country");
                // config.RegisterRepository<User, JsonRepository<User>>("user");
                config.RegisterRepository<TagGroup, JsonRepository<TagGroup>>("taggroup");
                config.RegisterRepository<Tag, JsonRepository<Tag>>("tag");
                config.RegisterRepository<MappedEntity, DatabaseEntity, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>("mapped")
                    .SetDataViewBuilder<DatabaseEntityDataViewBuilder>();

                config.RegisterFileUploadHandler<Base64TextFileUploadHandler>();
                config.RegisterFileUploadHandler<Base64ImageUploadHandler>();

                config.AllowAnonymousUser();
            });

            services.AddCors();
            services
                .AddControllers(config =>
                {
                    config.Conventions.AddRapidCMSRouteConvention();
                })
                .AddNewtonsoftJson()
                .ConfigureApplicationPartManager(configure =>
                {
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}
