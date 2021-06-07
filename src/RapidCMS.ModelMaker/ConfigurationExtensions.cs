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
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddModelMaker(
            this IServiceCollection services,
            bool addDefaultPropertiesAndValidators = true,
            Action<IModelMakerConfig>? configure = null)
        {
            services.AddTransient<IPlugin, ModelMakerPlugin>();

            services.AddTransient<CollectionsDataCollection>();
            services.AddTransient<PropertyEditorDataCollection>();
            services.AddTransient<PropertyTypeDataCollection>();

            services.AddScoped<ModelMakerRepository>();
            services.AddScoped<ModelRepository>();
            services.AddScoped<PropertyRepository>();

            services.AddTransient<BooleanLabelDataCollectionFactory>();
            services.AddTransient<LimitedOptionsDataCollectionFactory>();
            services.AddTransient<LinkedEntityDataCollectionFactory>();

            var config = new ModelMakerConfig();

            if (addDefaultPropertiesAndValidators)
            {
                services.AddTransient<BooleanLabelValidator>();
                services.AddTransient<LimitedOptionsValidator>();
                services.AddTransient<LinkedEntitiesValidator>();
                services.AddTransient<LinkedEntityValidator>();
                services.AddTransient<MaxLengthValidator>();
                services.AddTransient<MinLengthValidator>();

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
                    "The value has to be one of the entities of the linked collection",
                    EditorType.Dropdown,
                    x => x.Config.CollectionAlias);

                config.AddPropertyValidator<LinkedEntitiesValidator, List<string>, LinkedEntityValidationConfig, string, LinkedEntityDataCollectionFactory>(
                    Constants.Validators.LinkedEntities,
                    "Linked entity",
                    "The value has to be one or more of the entities of the linked collection",
                    EditorType.Dropdown,
                    x => x.Config.CollectionAlias);

                config.AddPropertyValidator<BooleanLabelValidator, bool, BooleanLabelValidationConfig, BooleanLabelValidationConfig.LabelsConfig, BooleanLabelDataCollectionFactory>(
                    Constants.Validators.BooleanLabels,
                    "Labels",
                    "The editor will display a dropdown instead of a checkbox",
                    EditorType.ModelEditor,
                    x => x.Config.Labels);

                config.AddPropertyEditor(Constants.Editors.Checkbox, "Checkbox", EditorType.Checkbox);
                config.AddPropertyEditor(Constants.Editors.Date, "Date", EditorType.Date);
                config.AddPropertyEditor(Constants.Editors.Dropdown, "Dropdown", EditorType.Dropdown);
                config.AddPropertyEditor(Constants.Editors.EntitiesPicker, "Entities Picker", EditorType.EntitiesPicker);
                config.AddPropertyEditor(Constants.Editors.EntityPicker, "Enitity Picker", EditorType.EntityPicker);
                config.AddPropertyEditor(Constants.Editors.MultiSelect, "Multi-select", EditorType.MultiSelect);
                config.AddPropertyEditor(Constants.Editors.Numeric, "Numeric", EditorType.Numeric);
                config.AddPropertyEditor(Constants.Editors.Select, "Select", EditorType.Select);
                config.AddPropertyEditor(Constants.Editors.TextArea, "Text area", EditorType.TextArea);
                config.AddPropertyEditor(Constants.Editors.TextBox, "Text box", EditorType.TextBox);

                config.AddProperty<string>(
                        Constants.Properties.ShortString,
                        "Short string",
                        "Label",
                        new[] { Constants.Editors.TextBox, Constants.Editors.TextArea, Constants.Editors.Dropdown },
                        new[] { Constants.Validators.MinLength, Constants.Validators.MaxLength, Constants.Validators.LimitedOptions });

                config.AddProperty<string>(
                        Constants.Properties.LongString,
                        "Long string",
                        "Label",
                        new[] { Constants.Editors.TextArea },
                        new[] { Constants.Validators.MinLength });

                config.AddProperty<bool>(
                        Constants.Properties.Boolean,
                        "Boolean",
                        "ToggleLeft",
                        new[] { Constants.Editors.Checkbox, Constants.Editors.Dropdown, Constants.Editors.Select },
                        new[] { Constants.Validators.BooleanLabels });

                config.AddProperty<string>(
                        Constants.Properties.LinkedEntity,
                        "Linked entity",
                        "Link",
                        new[] { Constants.Editors.EntityPicker },
                        new[] { Constants.Validators.LinkedEntity })
                    .CanBeUsedAsTitle(false);

                config.AddProperty<List<string>>(
                        Constants.Properties.LinkedEntities,
                        "Linked entities",
                        "Link",
                        new[] { Constants.Editors.EntitiesPicker },
                        new[] { Constants.Validators.LinkedEntities }) // TODO: min max linkedentities
                    .CanBeUsedAsTitle(false);

                config.AddProperty<DateTime>(
                        Constants.Properties.Date,
                        "Date",
                        "Calendar",
                        new[] { Constants.Editors.Date },
                        Enumerable.Empty<string>()); // TODO: date validation (valid ranges etc)

                config.AddProperty<double>(
                        Constants.Properties.Numeric,
                        "Number",
                        "NumberField",
                        new[] { Constants.Editors.Numeric },
                        Enumerable.Empty<string>())
                    .CanBeUsedAsTitle(false); // TODO: min max limitedoptions

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
