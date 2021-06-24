using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Collections;
using RapidCMS.ModelMaker.CommandHandlers;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Factories;
using RapidCMS.ModelMaker.Models;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.Repositories;
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
            // TODO:
            // v 4.0.0-preview: after implementing basic ModelEntity generation
            // v 4.0.1-preview: improved generation
            // v 4.0.2-preview: improved validation
            // - 4.0.3-preview: after implementing configurable sub collections + alias working
            // - 4.0.x-preview: finish other milestone tickets
            // - 4.0.x2-preview: get WebAssembly + APIs working + updated
            // - 4.0.0: after implementing complete DbContext generation by configured code

            // general TODO:
            // v move IPublishableEntity features to a separate UI package (it's not for ModelMaker anymore)
            // - implement complex validation like the old IValidator using validation pipeline + generated validators -- attribute validation is not enough for modelmakermade models
            // v configure collection icon + color
            // v configure single and plural name of collection
            // v configure nice names for properties
            // - configure collection shape like conventions based collections (list view + node editor / list editor / list view)
            // v configure what goes on the list view
            // - validate that a referenced collection has an entity that has an Id property of type int32
            // - add support for data collections from enums
            // v add flag editor for setting enum flag properties
            // v configure corresponding / reciprocal property for one-to-one, one-to-many, many-to-one and many-to-many
            // - fix search field from shifting left when picker is validated
            // - fix delete node and get redirected to error-error
            // - fix EntityValidator for API

            // docs:
            // general behavior:
            // one way linked entities are always one-to-many or many-to-many relations in EF Core

            services.AddTransient<IPlugin, ModelMakerPlugin>();

            services.AddTransient<CollectionsDataCollection>();
            services.AddTransient<PropertyEditorDataCollection>();
            services.AddTransient<PropertyTypeDataCollection>();

            services.AddScoped<ModelRepository>();
            services.AddScoped<PropertyRepository>();

            services.AddTransient<BooleanLabelDataCollectionFactory>();
            services.AddTransient<LimitedOptionsDataCollectionFactory>();
            services.AddTransient<LinkedEntityDataCollectionFactory>();
            services.AddTransient<ReciprocalPropertyCollectionFactory>();

            var config = new ModelMakerConfig();

            if (addDefaultPropertiesAndValidators)
            {
                //services.AddTransient<BooleanLabelValidator>();
                //services.AddTransient<LimitedOptionsValidator>();
                //services.AddTransient<LinkedEntitiesValidator>();
                //services.AddTransient<LinkedEntityValidator>();
                //services.AddTransient<MaxLengthValidator>();
                //services.AddTransient<MinLengthValidator>();

                config.AddPropertyValidator<string, MinLengthValidationConfig, int?>(
                    Constants.Validators.MinLength,
                    "Minimum length",
                    "The value has to be at least this amount of characters.",
                    EditorType.Numeric,
                    x => x.Config.MinLength);

                config.AddPropertyValidator<string, MaxLengthValidationConfig, int?>(
                    Constants.Validators.MaxLength,
                    "Maximum length",
                    "The value has to be at most this amount of characters.",
                    EditorType.Numeric,
                    x => x.Config.MaxLength);

                config.AddPropertyValidator<string, LimitedOptionsValidationConfig, IList<string>, LimitedOptionsDataCollectionFactory>(
                    Constants.Validators.LimitedOptions,
                    "Limited options",
                    "The value has to be one of these items",
                    EditorType.ListEditor,
                    x => x.Config.Options);

                config.AddPropertyValidator<string, LinkedEntityValidationConfig, string, LinkedEntityDataCollectionFactory>(
                    Constants.Validators.LinkedEntity,
                    "Linked entity",
                    "The value has to be one of the entities of the linked collection",
                    EditorType.Dropdown,
                    x => x.Config.LinkedEntityCollectionAlias);

                config.AddPropertyValidator<List<string>, LinkedEntitiesValidationConfig, string, LinkedEntityDataCollectionFactory>(
                    Constants.Validators.LinkedEntities,
                    "Linked entity",
                    "The value has to be one or more of the entities of the linked collection",
                    EditorType.Dropdown,
                    x => x.Config.LinkedEntitiesCollectionAlias);

                config.AddPropertyValidator<string, CorrespondingPropertyValidationConfig, string?, ReciprocalPropertyCollectionFactory>(
                    Constants.Validators.ReciprocalProperty,
                    "Corresponding property",
                    "The property of the relation on the other model. If kept empty, a hidden corresponding property will be created.",
                    EditorType.Dropdown,
                    x => x.Config.RelatedPropertyName);

                config.AddPropertyValidator<bool, BooleanLabelValidationConfig, BooleanLabelValidationConfig.LabelsConfig, BooleanLabelDataCollectionFactory>(
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
                        new[] { Constants.Validators.LinkedEntity, Constants.Validators.ReciprocalProperty })
                    .CanBeUsedAsTitle(false)
                    .RelatesToOneEntity(true);

                config.AddProperty<List<string>>(
                        Constants.Properties.LinkedEntities,
                        "Linked entities",
                        "Link",
                        new[] { Constants.Editors.EntitiesPicker },
                        new[] { Constants.Validators.LinkedEntities, Constants.Validators.ReciprocalProperty }) // TODO: min max linkedentities
                    .CanBeUsedAsTitle(false)
                    .RelatesToManyEntities(true);

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

            services.AddTransient<ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse>, RemoveModelEntityCommandHandler>();

            services.AddTransient<ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>>, GetAllModelEntitiesCommandHandler>();
            services.AddTransient<ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>>, GetModelEntityCommandHandler>();

            services.AddTransient<ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>>, InsertModelEntityCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse>, UpdateModelEntityCommandHandler>();
            services.AddTransient<ICommandHandler<PublishRequest<ModelEntity>, ConfirmResponse>, PublishModelEntityCommandHandler>();

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
