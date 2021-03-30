using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.CommandHandlers;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Models;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
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
        public const string MinLength = "minlength";
        public const string MaxLength = "maxlength";
        public const string LimitedOptions = "limitedOptions";

        public static IServiceCollection AddModelMaker(this IServiceCollection services, Action<IModelMakerConfig>? configure = null)
        {
            // TODO: what should this life time be?
            services.AddTransient<IPlugin, ModelMakerPlugin>();

            //services.AddTransient<ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse>, InMemoryModelEntityCommandHandler>();
            //services.AddTransient<ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>>, InMemoryModelEntityCommandHandler>();
            //services.AddTransient<ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>>, InMemoryModelEntityCommandHandler>();
            //services.AddTransient<ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>>, InMemoryModelEntityCommandHandler>();
            //services.AddTransient<ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>>, InMemoryModelEntityCommandHandler>();
            //services.AddTransient<ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse>, InMemoryModelEntityCommandHandler>();

            services.AddTransient<PropertyEditorDataCollection>();
            services.AddTransient<PropertyTypeDataCollection>();

            services.AddScoped<ModelMakerRepository>();
            services.AddScoped<ModelRepository>();
            services.AddScoped<PropertyRepository>();
            services.AddScoped<ValidationRepository>();

            services.AddTransient<MinLengthValidator>();
            services.AddTransient<MaxLengthValidator>();
            services.AddTransient<LimitedOptionsValidator>();

            var config = new ModelMakerConfig();

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

            config.AddPropertyValidator<LimitedOptionsValidator, string, LimitedOptionsValidationConfig, IList<string>>(
                LimitedOptions,
                "Limited options",
                "The value has to be one of these items",
                EditorType.TextBox,
                x => x.Config.Options); // TODO: convert to tag editor
            // TODO: custom type

            config.AddPropertyEditor(TextBox, "Text box", EditorType.TextBox);
            config.AddPropertyEditor(TextArea, "Text area", EditorType.TextArea);
            config.AddPropertyEditor(Dropdown, "Dropdown", EditorType.Dropdown);
            
            // TODO: add data collection to be configured

            config.AddProperty<string>("shortstring", "Short string", "Label", new[] { TextBox, TextArea, Dropdown }, new[] { MinLength, MaxLength, LimitedOptions });
            config.AddProperty<string>("longstring", "Long string", "Label", new[] { TextArea }, new[] { MinLength });

            configure?.Invoke(config);

            services.AddSingleton<IModelMakerConfig>(config);

            return services;
        }

        public static ICmsConfig AddModelMakerPlugin(this ICmsConfig cmsConfig)
        {
            cmsConfig.AddPlugin<ModelMakerPlugin>();

            cmsConfig.AddCollection<ModelEntity, ModelRepository>(
                "modelmakeradmin",
                "Database",
                "MagentaPink10",
                "Models",
                
                config =>
                {
                    config.SetTreeView(x => x.Name);

                    config.SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);

                        view.AddRow(row =>
                        {
                            row.AddField(x => x.Name);

                            row.AddDefaultButton(DefaultButtonType.Edit);
                        });
                    });

                    config.SetNodeEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.Up);
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                        editor.AddDefaultButton(DefaultButtonType.SaveNew);
                        editor.AddDefaultButton(DefaultButtonType.Delete);

                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Name);
                            section.AddField(x => x.Alias);

                            section.AddSubCollectionList("modelmaker::property");
                        });
                    });
                });

            return cmsConfig;
        }
    }
}
