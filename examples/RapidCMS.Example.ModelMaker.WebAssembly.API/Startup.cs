using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.ModelMaker;

namespace RapidCMS.Example.ModelMaker.WebAssembly.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<BlogRepository>();
            services.AddScoped<CategoryRepository>();

            services.AddTransient<BlogValidator>();
            services.AddTransient<CategoryValidator>();

            services.AddDbContext<ModelMakerDbContext>(
                builder => builder.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString")),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient);

            services.AddRapidCMSWebApi(config =>
            {
                config.AllowAnonymousUser();

                config.RegisterEntityValidator<Blog, BlogValidator>();
                config.RegisterEntityValidator<Category, CategoryValidator>();

                config.RegisterRepository<Blog, BlogRepository>();
                config.RegisterRepository<Category, CategoryRepository>();
            });

            services.AddCors();

            services.AddRapidCMSControllers(config =>
            {
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

