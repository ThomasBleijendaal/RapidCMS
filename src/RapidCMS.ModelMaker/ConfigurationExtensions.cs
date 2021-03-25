using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Providers;
using RapidCMS.Repositories;

namespace RapidCMS.ModelMaker
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddModelMaker(this IServiceCollection services)
        {
            // TODO: what should this life time be?
            services.AddTransient<IPlugin, ModelMakerPlugin>();

            services.AddScoped<ModelMakerRepository>();
            services.AddScoped<JsonRepository<ModelConfigurationEntity>>();

            return services;
        }

        public static ICmsConfig AddModelMakerPlugin(this ICmsConfig cmsConfig)
        {
            cmsConfig.AddPlugin<ModelMakerPlugin>();

            cmsConfig.AddCollection<ModelConfigurationEntity, JsonRepository<ModelConfigurationEntity>>(
                "modelmakerconfiguration",
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
                            section.AddField(x => x.Type)
                                .SetType(EditorType.Dropdown)
                                .SetDataCollection<EnumDataProvider<ModelPropertyType>>();
                        });
                    });
                });

            return cmsConfig;
        }
    }

    public class ModelConfigurationEntity : IEntity, ICloneable
    {
        public string? Id { get; set; }

        public string Name { get; set; } = default!;

        // move to property
        public ModelPropertyType Type { get; set; } = default!;

        public List<ModelProperty> Properties { get; set; } = new List<ModelProperty>();

        public object Clone()
        {
            return new ModelConfigurationEntity
            {
                Id = Id,
                Name = Name,
                Properties = Properties
            };
        }
    }

    public class ModelProperty
    {
        public ModelPropertyType Type { get; set; } = default!;


    }

    public enum ModelPropertyType
    {
        String
    }
}
