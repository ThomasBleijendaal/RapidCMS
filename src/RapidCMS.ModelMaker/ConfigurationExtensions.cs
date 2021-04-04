using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Collections;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Models;
using RapidCMS.ModelMaker.Repositories;
using RapidCMS.ModelMaker.Validation;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker
{
    public static class ConfigurationExtensions
    {   
        public const string TextBox = "textbox";
        public const string TextArea = "textarea";
        public const string Dropdown = "dropdown";
        public const string Numeric = "numeric";
        public const string Checkbox = "checkbox";
        public const string MinLength = "minlength";
        public const string MaxLength = "maxlength";
        public const string LimitedOptions = "limitedOptions";

        public static IServiceCollection AddModelMaker(
            this IServiceCollection services, 
            bool addDefaultPropertiesAndValidators = true,
            Action<IModelMakerConfig>? configure = null)
        {
            // TODO: what should this life time be?
            services.AddTransient<IPlugin, ModelMakerPlugin>();

            services.AddTransient<PropertyEditorDataCollection>();
            services.AddTransient<PropertyTypeDataCollection>();

            services.AddScoped<ModelMakerRepository>();
            services.AddScoped<ModelRepository>();
            services.AddScoped<PropertyRepository>();

            var config = new ModelMakerConfig();

            if (addDefaultPropertiesAndValidators)
            {
                services.AddTransient<MinLengthValidator>();
                services.AddTransient<MaxLengthValidator>();
                services.AddTransient<LimitedOptionsValidator>();

                config.AddPropertyValidator<MinLengthValidator, string, MinLengthValidationConfig, int?>(
                    MinLength,
                    "Minimum length",
                    "The value has to be at least this amount of characters.",
                    EditorType.Numeric,
                    x => x.Config.MinLength);

                config.AddPropertyValidator<MaxLengthValidator, string, MaxLengthValidationConfig, int?>(
                    MaxLength,
                    "Maximum length",
                    "The value has to be at most this amount of characters.",
                    EditorType.Numeric,
                    x => x.Config.MaxLength);

                config.AddPropertyValidator<LimitedOptionsValidator, string, LimitedOptionsValidationConfig, IList<string>, ModelMakerLimitedOptionsDataCollection>(
                    LimitedOptions,
                    "Limited options",
                    "The value has to be one of these items",
                    EditorType.ListEditor,
                    x => x.Config.Options);
                // TODO: custom type

                config.AddPropertyEditor(TextBox, "Text box", EditorType.TextBox);
                config.AddPropertyEditor(TextArea, "Text area", EditorType.TextArea);
                config.AddPropertyEditor(Dropdown, "Dropdown", EditorType.Dropdown);
                config.AddPropertyEditor(Numeric, "Numeric", EditorType.Numeric);
                config.AddPropertyEditor(Checkbox, "Checkbox", EditorType.Checkbox);

                config.AddProperty<string>("shortstring", "Short string", "Label", new[] { TextBox, TextArea, Dropdown }, new[] { MinLength, MaxLength, LimitedOptions });
                config.AddProperty<string>("longstring", "Long string", "Label", new[] { TextArea }, new[] { MinLength });
                // TODO: slug, media, reference(s) (model maker / external), markdown, JSON object, date, date time, time, rich text
            }

            configure?.Invoke(config);

            services.AddSingleton<IModelMakerConfig>(config);

            return services;
        }

        public static ICmsConfig AddModelMakerPlugin(this ICmsConfig cmsConfig)
        {
            cmsConfig.AddPlugin<ModelMakerPlugin>();

            cmsConfig.AddModelCollection();

            return cmsConfig;
        }
    }
}
