using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Example.Collections;
using RapidCMS.Example.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example
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

            services.AddSingleton<JsonRepository<Person>>();
            services.AddSingleton<JsonRepository<Country>>();
            services.AddSingleton<JsonRepository<User>>();
            services.AddSingleton<JsonRepository<TagGroup>>();
            services.AddSingleton<JsonRepository<Tag>>();

            services.AddRapidCMS(config =>
            {
                config.AllowAnonymousUser();

                // CRUD editor for simple POCO with recursive sub collections
                // --> see Collections/PersonCollection for the basics of this CMS
                config.AddPersonCollection();

                // CRUD editor with support for one-to-many relation + validation
                // --> see Collections/CountryCollection for one-to-many relation with validation
                config.AddCountryCollection();

                // CRUD editor with validation attributes, custom editor and custom button panes
                // --> see Collections/UserCollection 
                config.AddUserCollection();

                // CRUD editor with nested collection
                // --> see Collections/TagCollection
                config.AddTagCollection();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
