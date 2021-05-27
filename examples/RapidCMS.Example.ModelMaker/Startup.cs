using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Data;
using RapidCMS.ModelMaker;
using RapidCMS.ModelMaker.TableStorage;
using RapidCMS.Repositories;

namespace RapidCMS.Example.ModelMaker
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
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddModelMaker(
                // this flag adds the default properties and validators covering most of the use cases
                // each of the features added can also be added manually, to fully control the feature set of the model maker
                // (see ConfigurationExtensions.cs of RapidCMS.ModelMaker to see what is added when this flag is true)
                addDefaultPropertiesAndValidators: true,
                config =>
                {
                    
                });
            services.AddModelMakerTableStorage();

            services.AddScoped<BaseRepository<Person>, JsonRepository<Person>>();
            services.AddScoped<BaseRepository<Details>, JsonRepository<Details>>();

            services.AddRapidCMSServer(config =>
            {
                config.AllowAnonymousUser();

                config.SetSiteName("Model maker");

                config.AddPersonCollection();

                config.AddModelMakerPlugin();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRapidCMS(isDevelopment: env.IsDevelopment());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
