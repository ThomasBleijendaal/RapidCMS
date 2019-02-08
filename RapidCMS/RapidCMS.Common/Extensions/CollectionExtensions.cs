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

    public static class RootExtensions
    {
        public static void AddCollection<TEntity>(this Root root, string alias, string name, Action<CollectionConfig<TEntity>> configure)
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
                var prop = GetterAndSetterHelper.Create(configReceiver.TreeView.NameGetter);

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

            collection.SubCollections = configReceiver.SubCollections;
            
            root.Collections.Add(collection);
        }
    }
}
