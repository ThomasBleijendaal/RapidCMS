using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Collections;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Factories;
using RapidCMS.ModelMaker.Models;
using RapidCMS.ModelMaker.Repositories;
using RapidCMS.ModelMaker.Validation;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker
{
    public static class Constants
    {
        public static class Editors
        {
            public const string TextBox = "textbox";
            public const string TextArea = "textarea";
            public const string Dropdown = "dropdown";
            public const string Numeric = "numeric";
            public const string Checkbox = "checkbox";
            public const string Date = "date";
            public const string Select = "select";
            public const string MultiSelect = "multiselect";
        }

        public static class Validators
        {
            public const string MinLength = "minlength";
            public const string MaxLength = "maxlength";
            public const string LimitedOptions = "limitedOptions";
            public const string LinkedEntity = "linkedEntity";
        }
    }

    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddModelMaker(
            this IServiceCollection services,
            bool addDefaultPropertiesAndValidators = true,
            Action<IModelMakerConfig>? configure = null)
        {
            // TODO: what should this lifetime be?
            services.AddTransient<IPlugin, ModelMakerPlugin>();

            services.AddTransient<CollectionsDataCollection>();
            services.AddTransient<PropertyEditorDataCollection>();
            services.AddTransient<PropertyTypeDataCollection>();

            services.AddScoped<ModelMakerRepository>();
            services.AddScoped<ModelRepository>();
            services.AddScoped<PropertyRepository>();

            services.AddTransient<LimitedOptionsDataCollectionFactory>();
            services.AddTransient<LinkedEntityDataCollectionFactory>();

            var config = new ModelMakerConfig();

            if (addDefaultPropertiesAndValidators)
            {
                services.AddTransient<MinLengthValidator>();
                services.AddTransient<MaxLengthValidator>();
                services.AddTransient<LimitedOptionsValidator>();
                services.AddTransient<LinkedEntityValidator>();

                config.AddPropertyValidator<MinLengthValidator, string, MinLengthValidationConfig, int?>(
                    Constants.Validators.MinLength,
                    "Minimum length",
                    "The value has to be at least this amount of characters.",
                    EditorType.Numeric,
                    x => x.Config.MinLength);

                config.AddPropertyValidator<MaxLengthValidator, string, MaxLengthValidationConfig, int?>(
                    Constants.Validators.MaxLength,
                    "Maximum length",
                    "The value has to be at most this amount of characters.",
                    EditorType.Numeric,
                    x => x.Config.MaxLength);

                config.AddPropertyValidator<LimitedOptionsValidator, string, LimitedOptionsValidationConfig, IList<string>, LimitedOptionsDataCollectionFactory>(
                    Constants.Validators.LimitedOptions,
                    "Limited options",
                    "The value has to be one of these items",
                    EditorType.ListEditor,
                    x => x.Config.Options);

                config.AddPropertyValidator<LinkedEntityValidator, string, LinkedEntityValidationConfig, string, LinkedEntityDataCollectionFactory>(
                    Constants.Validators.LinkedEntity,
                    "Linked entity",
                    "The value has to be one of entities of the linked collection",
                    EditorType.Dropdown,
                    x => x.Config.CollectionAlias);

                // TODO: allow validators to hide themselves when property is not good for them (max length on dropdown for example)
                // TODO: validation configuration validation
                // TODO: custom type

                config.AddPropertyEditor(Constants.Editors.Checkbox, "Checkbox", EditorType.Checkbox);
                config.AddPropertyEditor(Constants.Editors.Date, "Date", EditorType.Date);
                config.AddPropertyEditor(Constants.Editors.Dropdown, "Dropdown", EditorType.Dropdown);
                config.AddPropertyEditor(Constants.Editors.MultiSelect, "Multi-select", EditorType.MultiSelect);
                config.AddPropertyEditor(Constants.Editors.Numeric, "Numeric", EditorType.Numeric);
                config.AddPropertyEditor(Constants.Editors.Select, "Select", EditorType.Select);
                config.AddPropertyEditor(Constants.Editors.TextArea, "Text area", EditorType.TextArea);
                config.AddPropertyEditor(Constants.Editors.TextBox, "Text box", EditorType.TextBox);
                
                config.AddProperty<string>(
                    "shortstring",
                    "Short string",
                    "Label",
                    new[] { Constants.Editors.TextBox, Constants.Editors.TextArea, Constants.Editors.Dropdown },
                    new[] { Constants.Validators.MinLength, Constants.Validators.MaxLength, Constants.Validators.LimitedOptions });

                config.AddProperty<string>(
                    "longstring",
                    "Long string",
                    "Label",
                    new[] { Constants.Editors.TextArea },
                    new[] { Constants.Validators.MinLength });

                config.AddProperty<bool>(
                    "boolean",
                    "Boolean",
                    "ToggleLeft",
                    new[] { Constants.Editors.Checkbox },
                    Enumerable.Empty<string>()); // TODO: dropdown with labels for true / false

                config.AddProperty<string>(
                    "linkedentity", 
                    "Linked entity", 
                    "Link", 
                    new[] { Constants.Editors.Dropdown, Constants.Editors.Select }, 
                    new[] { Constants.Validators.LinkedEntity });

                

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
