using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example.WebAssembly.FunctionAPI
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
            services.AddScoped<JsonRepository<Person>>();
            services.AddScoped<JsonRepository<Details>>();

            services.AddRapidCMSFunctions(config =>
            {
                if (!ConfigureAuthentication)
                {
                    config.AllowAnonymousUser();
                }

                config.RegisterRepository<Person, JsonRepository<Person>>();
                config.RegisterRepository<Details, JsonRepository<Details>>();
            });
        }

        public void ConfigureWorker(IFunctionsWorkerApplicationBuilder builder)
        {
            builder.UseFunctionExecutionMiddleware();
            // TODO: add authentication middleware
        }
    }
}
