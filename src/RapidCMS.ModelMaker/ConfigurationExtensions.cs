﻿using System;
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
            // v 4.0.3-preview: completed validation
            // v 4.0.4-preview: all customizations working + generated correctly
            // v 4.0.x-preview: finish other milestone tickets
            // v 4.0.x2-preview: get WebAssembly + APIs working + updated
            // - 4.0.0: after implementing complete DbContext generation by configured code + final fixes below + docs updated
            // - 4.1.0-preview: after implementing configurable sub collections + mixing modelmaker + non-modelmaker collections

            // general TODO:
            // - validate that a referenced collection has an entity that has an Id property of type int32
            // - configure collection shape like conventions based collections (list view + node editor / list editor / list view)
            // - fix search field from shifting left when picker is validated
            // - fix delete node and get redirected to error-error
            // - allow for disabling model maker without losing stuff like BooleanLabelDataCollection (for production deployment purposes)
            // - restore max length attribute for nvarchar fix + required for relations
            // - add support for file upload

            // docs:
            // general behavior:
            // one way linked entities are always one-to-many or many-to-many relations in EF Core

            services.AddTransient<IPlugin, ModelMakerPlugin>();

            services.AddTransient<BooleanLabelDataCollection>();
            services.AddTransient<CollectionsDataCollection>();
            services.AddTransient<PropertyEditorDataCollection>();
            services.AddTransient<PropertyTypeDataCollection>();
            services.AddTransient<ReciprocalPropertyDataCollection>();

            services.AddScoped<ModelRepository>();
            services.AddScoped<PropertyRepository>();

            var config = new ModelMakerConfig();

            if (addDefaultPropertiesAndValidators)
            {
                config.AddPropertyDetail<MinLengthDetailConfig, int?>(
                    Constants.PropertyDetails.MinLength,
                    "Minimum length",
                    "The value has to be at least this amount of characters.",
                    EditorType.Numeric,
                    x => x.Config.MinLength);

                config.AddPropertyDetail<MaxLengthDetailConfig, int?>(
                    Constants.PropertyDetails.MaxLength,
                    "Maximum length",
                    "The value has to be at most this amount of characters.",
                    EditorType.Numeric,
                    x => x.Config.MaxLength);

                config.AddPropertyDetail<LimitedOptionsDetailConfig, IList<string>>(
                    Constants.PropertyDetails.LimitedOptions,
                    "Limited options",
                    "The value has to be one of these items",
                    EditorType.ListEditor,
                    x => x.Config.Options);

                config.AddPropertyDetail<LinkedEntityDetailConfig, string, CollectionsDataCollection>(
                    Constants.PropertyDetails.LinkedEntity,
                    "Linked entity",
                    "The value has to be one of the entities of the linked collection",
                    EditorType.Dropdown,
                    x => x.Config.LinkedEntityCollectionAlias);

                config.AddPropertyDetail<LinkedEntitiesDetailConfig, string, CollectionsDataCollection>(
                    Constants.PropertyDetails.LinkedEntities,
                    "Linked entity",
                    "The value has to be one or more of the entities of the linked collection",
                    EditorType.Dropdown,
                    x => x.Config.LinkedEntitiesCollectionAlias);

                config.AddPropertyDetail<CorrespondingPropertyDetailConfig, string?, ReciprocalPropertyDataCollection>(
                    Constants.PropertyDetails.ReciprocalProperty,
                    "Corresponding property",
                    "The property of the relation on the other model. If kept empty, a hidden corresponding property will be created.",
                    EditorType.Dropdown,
                    x => x.Config.RelatedPropertyName);

                config.AddPropertyDetail<BooleanLabelDetailConfig, BooleanLabelDetailConfig.LabelsConfig>(
                    Constants.PropertyDetails.BooleanLabels,
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
                        new[] { Constants.PropertyDetails.MinLength, Constants.PropertyDetails.MaxLength, Constants.PropertyDetails.LimitedOptions });

                config.AddProperty<string>(
                        Constants.Properties.LongString,
                        "Long string",
                        "Label",
                        new[] { Constants.Editors.TextArea },
                        new[] { Constants.PropertyDetails.MinLength });

                config.AddProperty<bool>(
                        Constants.Properties.Boolean,
                        "Boolean",
                        "ToggleLeft",
                        new[] { Constants.Editors.Checkbox, Constants.Editors.Dropdown, Constants.Editors.Select },
                        new[] { Constants.PropertyDetails.BooleanLabels });

                config.AddProperty<string>(
                        Constants.Properties.LinkedEntity,
                        "Linked entity",
                        "Link",
                        new[] { Constants.Editors.EntityPicker },
                        new[] { Constants.PropertyDetails.LinkedEntity, Constants.PropertyDetails.ReciprocalProperty })
                    .CanBeUsedAsTitle(false)
                    .RelatesToOneEntity(true);

                config.AddProperty<List<string>>(
                        Constants.Properties.LinkedEntities,
                        "Linked entities",
                        "Link",
                        new[] { Constants.Editors.EntitiesPicker },
                        new[] { Constants.PropertyDetails.LinkedEntities, Constants.PropertyDetails.ReciprocalProperty }) // TODO: min max linkedentities
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
