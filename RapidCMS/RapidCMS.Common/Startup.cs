using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Models;
using RapidCMS.Common.Services;

namespace RapidCMS.Common
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICollectionService, CollectionService>();
        }

        //public void Configure(IApplicationBuilder app)
        public void Configure(IComponentsApplicationBuilder app)
        {
            try
            {
                var root = app.Services.GetService<Root>();

                root.MaterializeRepositories(app.Services);

                // TODO: populate value mappers
            }
            catch (Exception ex)
            {

            }
        }
    }
}
