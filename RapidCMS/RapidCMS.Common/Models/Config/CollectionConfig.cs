using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using RapidCMS.Common.Data;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models.Config
{
    // TODO: validate incoming parameters
    public class CollectionConfig<TEntity>
        where TEntity : IEntity
    {
        internal Type RepositoryType { get; set; }   

        public TreeViewConfig<TEntity> TreeView { get; set; }

        public ListViewConfig<TEntity> ListView { get; set; }

        public List<Collection> SubCollections { get; set; } = new List<Collection>();

        public CollectionConfig<TEntity> SetRepository<TRepository>()
           where TRepository : IRepository<int, TEntity>
        {
            RepositoryType = typeof(TRepository);

            return this;
        }

        public CollectionConfig<TEntity> SetTreeView(string name, ViewType viewType, Expression<Func<TEntity, string>> nameExpression)
        {
            TreeView = new TreeViewConfig<TEntity>
            {
                Name = name,
                ViewType = viewType,
                NameGetter = nameExpression
            };

            return this;
        }

        public CollectionConfig<TEntity> SetListView(Action<ListViewConfig<TEntity>> configure)
        {
            var config = new ListViewConfig<TEntity>();

            configure.Invoke(config);

            ListView = config;

            return this;
        }

        public CollectionConfig<TEntity> AddSubCollection<TSubEntity>(string alias, string name, Action<CollectionConfig<TEntity>> configure)
            where TSubEntity : IEntity
        {
            // TODO: merge this function RootExtensions.AddCollection
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

            SubCollections.Add(collection);

            return this;
        }
    }

    public class TreeViewConfig<TEntity>
        where TEntity : IEntity
    {
        public string Name { get; set; }
        public ViewType ViewType { get; set; }
        public Expression<Func<TEntity, string>> NameGetter { get; set; }
    }

    public class ListViewConfig<TEntity>
        where TEntity : IEntity
    {
        public List<ListViewPaneConfig<TEntity>> ListViewPanes { get; set; } = new List<ListViewPaneConfig<TEntity>>();

        public ListViewConfig<TEntity> AddListPane(Action<ListViewPaneConfig<TEntity>> configure)
        {
            var config = new ListViewPaneConfig<TEntity>();

            configure.Invoke(config);

            ListViewPanes.Add(config);

            return this;
        }
    }

    public class ListViewPaneConfig<TEntity>
        where TEntity : IEntity
    {
        public List<PropertyConfig<TEntity>> Properties { get; set; } = new List<PropertyConfig<TEntity>>();
        
        public PropertyConfig<TEntity> AddProperty<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<PropertyConfig<TEntity>> configure = null)
        {
            var config = new PropertyConfig<TEntity>
            {
                GetterAndSetter = GetterAndSetterHelper.Create(propertyExpression)
            };
            config.Name = config.GetterAndSetter.PropertyName;

            configure?.Invoke(config);

            Properties.Add(config);

            return config;
        }
    }

    public class PropertyConfig<TEntity>
        where TEntity : IEntity
    {
        internal string Name { get; set; }
        internal string Description { get; set; }

        internal GetterAndSetter GetterAndSetter { get; set; }
        internal Func<object, string> Formatter { get; set; } = (o) => $"{o}";

        public PropertyConfig<TEntity> SetName(string name)
        {
            Name = name;
            return this;
        }
        public PropertyConfig<TEntity> SetDescription(string description)
        {
            Description = description;
            return this;
        }
    }
}
