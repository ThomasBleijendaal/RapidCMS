using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Models;

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

            services.AddTransient<PropertyEditorDataCollection>();
            services.AddTransient<PropertyTypeDataCollection>();

            services.AddScoped<ModelMakerRepository>();
            services.AddScoped<ModelRepository>();
            services.AddScoped<PropertyRepository>();
            services.AddScoped<ValidationRepository>();

            var config = new ModelMakerConfig();

            config.AddPropertyValidator<MinLengthValidator, string, MinLengthValidationConfig>(
                MinLength,
                "Minimum length",
                "The value has to be at least this amount of characters.",
                EditorType.Numeric);

            config.AddPropertyValidator<MaxLengthValidator, string, MaxLengthValidationConfig>(
                MaxLength,
                "Maximum length",
                "The value has to be at most this amount of characters.",
                EditorType.Numeric);

            config.AddPropertyValidator<LimitedOptionsValidator, string, LimitedOptionsValidationConfig>(
                LimitedOptions,
                "Limited options",
                "The value has to be one of these items",
                EditorType.TextBox); // TODO: convert to tag editor
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




            var nameShortString = new PropertyModel
            {
                Name = "Name",
                PropertyAlias = "shortstring",
                EditorAlias = TextBox,
                Validations = new List<PropertyValidationModel>
                {
                    new PropertyValidationModel
                    {
                        Alias = MinLength,
                        Config = new MinLengthValidationConfig
                        {
                            MinLength = 3
                        }
                    }
                }
            };

            var shortString = new PropertyModelDescriptor
            {
                Alias = "shortstring",
                Editors = new List<PropertyEditorDescriptor>
                {
                    new PropertyEditorDescriptor
                    {
                        Alias = TextBox
                    },
                    new PropertyEditorDescriptor
                    {
                        Alias = TextArea
                    },
                    new PropertyEditorDescriptor
                    {
                        Alias = Dropdown
                    },
                },
                ValidatorAliases = new List<string>
                {
                    MinLength,
                    "LimitedOptions"
                }
            };

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

                            section.AddSubCollectionList("modelmaker::property");
                        });
                    });
                });

            return cmsConfig;
        }
    }
}
