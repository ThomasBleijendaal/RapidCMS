using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.ModelMaker.Components;
using RapidCMS.Example.ModelMaker.Models.Enums;
using RapidCMS.Example.ModelMaker.Models.Validators;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Data;
using RapidCMS.ModelMaker;
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
                    config.SetModelFolder("../RapidCMS.Example.ModelMaker.Models/RapidModels/");

                    // extra editors can be inserted easily
                    var customTextAreaEditor = config.AddPropertyEditor<CustomTextAreaEditor>("ctae", "Big textarea");

                    // adding new editors to existing properties is possible
                    config.GetProperty(Constants.Properties.ShortString)?.Editors.Add(customTextAreaEditor);

                    // custom editor validators can be inserted easily
                    var customTextValidator = config.AddPropertyDetail<BannedContentValidationConfig, List<string>>(
                        "bannedContent",
                        "Banned Content",
                        "The content is not allowed contain the following words.",
                        EditorType.ListEditor,
                        property => property.Config.BannedWords);

                    // adding extra validations to existing properties is possible
                    config.GetProperty(Constants.Properties.ShortString)?.Details.Add(customTextValidator);
                    config.GetProperty(Constants.Properties.LongString)?.Details.Add(customTextValidator);

                    // adding custom properties is also possible
                    var enumDropdownDetail = config.AddPropertyDetail<EnumDataProvider<ContentType>> (
                        "contentTypeValidator",
                        "Content Type",
                        "Content Type");

                    config.AddProperty<ContentType>(
                        "contentType",
                        "Content Type",
                        "Tag",
                        new[] { Constants.Editors.Dropdown, Constants.Editors.Select },
                        new[] { enumDropdownDetail.Alias });
                });

            services.AddScoped<BaseRepository<Details>, JsonRepository<Details>>();

            services.AddScoped<BaseRepository<Identity>, IdentityRepository>();
            

            //// TODO: add generator to automatically add this to DI
            services.AddTransient<IdentityValidator>();
            services.AddDbContext<ModelMakerDbContext>(
                builder => builder.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString")),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient);

            services.AddRapidCMSServer(config =>
            {
                config.AllowAnonymousUser();

                config.SetSiteName("Model maker");


                config.AddIdentityCollection();

                config.AddModelMakerPlugin();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ModelMakerDbContext context)
        {
            context.Database.Migrate();

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
