using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Data;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.Services;

namespace RapidCMS.Common.Extensions
{
    public static class RapidCMSMiddleware
    {
        public static IServiceCollection AddRapidCMS(this IServiceCollection services, Action<Root> configure)
        {
            var root = new Root();

            configure.Invoke(root);

            services.AddSingleton(root);

            return services;
        }
    }

    public static class ICollectionRootExtensions
    {
        public static ICollectionRoot AddCollection<TEntity>(this ICollectionRoot root, string alias, string name, Action<CollectionConfig<TEntity>> configure)
            where TEntity : IEntity
        {
            var collection = new Collection
            {
                Name = name,
                Alias = alias
            };

            var configReceiver = new CollectionConfig<TEntity>();

            configure.Invoke(configReceiver);

            collection.RepositoryType = configReceiver.RepositoryType;

            if (configReceiver.TreeView != null)
            {
                var prop = PropertyMetadataHelper.Create(configReceiver.TreeView.NameGetter);

                collection.TreeView = new TreeView
                {
                    Name = configReceiver.TreeView.Name,
                    EntityViewType = configReceiver.TreeView.ViewType,
                    NameGetter = prop.Getter
                };
            }

            if (configReceiver.ListView != null)
            {
                collection.ListView = new ListView
                {
                    ViewPanes = configReceiver.ListView.ListViewPanes.Select(pane =>
                    {
                        return new ViewPane<ListViewProperty>
                        {
                            Properties = pane.Properties.Select(property => new ListViewProperty
                            {
                                Description = property.Description,
                                Formatter = property.Formatter,
                                Getter = property.GetterAndSetter.Getter,
                                Name = property.Name
                            }).ToList()
                        };
                    }).ToList()
                };
            }

            if (configReceiver.NodeEditor != null)
            {
                collection.NodeEditor = new NodeEditor
                {
                    EditorPanes = configReceiver.NodeEditor.EditorPanes.Select(pane =>
                    {
                        return new EditorPane<Field>
                        {
                            Fields = pane.Fields.Select(field => new Field
                            {
                                DataType = field.Type,
                                Description = field.Description,
                                Getter = field.GetterAndSetter.Getter,
                                Name = field.Name,
                                Setter = field.GetterAndSetter.Setter
                            }).ToList()
                        };
                    }).ToList()
                };
            }

            collection.Collections = configReceiver.Collections;
            
            root.Collections.Add(collection);

            return root;
        }
    }
}
