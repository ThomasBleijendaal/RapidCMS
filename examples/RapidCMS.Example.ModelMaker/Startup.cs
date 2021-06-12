using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.ModelMaker.Components;
using RapidCMS.Example.ModelMaker.Enums;
using RapidCMS.Example.ModelMaker.Factories;
using RapidCMS.Example.ModelMaker.Validators;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Data;
using RapidCMS.ModelMaker;
using RapidCMS.ModelMaker.TableStorage;
using RapidCMS.ModelMaker.Validation;
using RapidCMS.ModelMaker.Validation.Config;
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
                    // extra editors can be inserted easily
                    var customTextAreaEditor = config.AddPropertyEditor<CustomTextAreaEditor>("ctae", "Big textarea");

                    // adding new editors to existing properties is possible
                    config.GetProperty(Constants.Properties.ShortString)?.Editors.Add(customTextAreaEditor);

                    // custom editor validators can be inserted easily
                    var customTextValidator = config.AddPropertyValidator<string, BannedContentValidationConfig, List<string>>(
                        "bannedContent",
                        "Banned Content",
                        "The content is not allowed contain the following words.",
                        EditorType.ListEditor,
                        property => property.Config.BannedWords);

                    // adding extra validations to existing properties is possible
                    config.GetProperty(Constants.Properties.ShortString)?.Validators.Add(customTextValidator);
                    config.GetProperty(Constants.Properties.LongString)?.Validators.Add(customTextValidator);

                    // adding custom properties is also possible
                    var enumDropdownValidator = config.AddPropertyValidator<ContentType, NoConfig, NoConfig, EnumOptionsDataCollectionFactory<ContentType>>(
                        "contentTypeValidator",
                        "Content Type",
                        "Content Type",
                        EditorType.None,
                        property => property.Config);

                    // TODO: enums gets eaten up by the json serializer
                    config.AddProperty<ContentType>(
                        "contentType",
                        "Content Type",
                        "Tag",
                        new[] { Constants.Editors.Dropdown, Constants.Editors.Select },
                        new[] { enumDropdownValidator.Alias });
                });
            services.AddModelMakerTableStorage();

            // validators are resolved from DI
            //services.AddSingleton<BannedContentValidator>();
            //services.AddSingleton<EnumOptionsValidator<ContentType>>();

            // factories are also resolved from DI
            services.AddSingleton<EnumOptionsDataCollectionFactory<ContentType>>();

            services.AddScoped<BaseRepository<Person>, JsonRepository<Person>>();
            services.AddScoped<BaseRepository<Details>, JsonRepository<Details>>();

            services.AddScoped<BaseRepository<Blog>, TempTestInMemoryRepository<Blog>>();

            services.AddRapidCMSServer(config =>
            {
                config.AllowAnonymousUser();

                config.SetSiteName("Model maker");

                config.AddBlogCollection();
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
