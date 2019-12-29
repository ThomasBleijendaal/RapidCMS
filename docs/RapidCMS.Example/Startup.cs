using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Common.Data;
using RapidCMS.Example.ActionHandlers;
using RapidCMS.Example.Collections;
using RapidCMS.Example.Components;
using RapidCMS.Example.Data;
using RapidCMS.Example.DataViews;
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

            services.AddSingleton<MappedInMemoryRepository<MappedEntity, DatabaseEntity>>();
            services.AddSingleton<IConverter<MappedEntity, DatabaseEntity>>(new Mapper());
            services.AddSingleton<DatabaseEntityDataViewBuilder>();

            services.AddSingleton<RandomNameActionHandler>();

            services.AddRapidCMS(config =>
            {
                config.AllowAnonymousUser();

                config.SetCustomLoginStatus(typeof(LoginStatus));

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

                // CRUD editor with entity mapping
                config.AddMappedCollection();

                // the dashboard can be build up of custom Blazor components, or the ListViews or ListEditors of collections
                config.AddDashboardSection(typeof(DashboardSection));
                config.AddDashboardSection("user", edit: true);
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

    public class Mapper : IConverter<MappedEntity, DatabaseEntity>
    {
        public MappedEntity Convert(DatabaseEntity obj)
        {
            return new MappedEntity
            {
                Description = obj.Description,
                Id = obj.Id,
                Name = obj.Name
            };
        }

        public DatabaseEntity Convert(MappedEntity obj)
        {
            return new DatabaseEntity
            {
                Description = obj.Description,
                Id = obj.Id,
                Name = obj.Name
            };
        }
    }
}
