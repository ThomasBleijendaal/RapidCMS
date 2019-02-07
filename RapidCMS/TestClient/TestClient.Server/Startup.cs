using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using TestLibrary;

namespace TestClient.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var hacky = new RapidCMS.Common.Startup();

            services.AddSingleton<TestRepository>();

            services.AddRapidCMS(root =>
            {
                root.AddCollection<TestEntity>("Collection 1", collection =>
                {
                    collection
                        .SetRepository<TestRepository>()
                        .SetTreeView("Tree", ViewType.List, entity => entity.Name);
                });

                //root.AddCollection("Collection 2", collection =>
                //{

                //});
            });
            
            services.AddRazorComponents<App.Startup>();
            
            // TODO: 
            hacky.ConfigureServices(services);
            //services.AddRazorComponents<RapidCMS.Common.Startup>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // TODO: 
            // app.UseRazorComponents<RapidCMS.Common.Startup>();
            var root = app.ApplicationServices.GetService<Root>();
            root.MaterializeRepositories(app.ApplicationServices);
            
            app.UseStaticFiles();
            app.UseRazorComponents<App.Startup>();

        }
    }
}
