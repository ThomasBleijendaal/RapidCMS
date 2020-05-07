using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config.Convention;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Config
{
    internal class CollectionConfig : ICollectionConfig
    {
        protected List<ICollectionConfig> _collections = new List<ICollectionConfig>();

        internal CollectionConfig(string alias, string? icon, string name, Type repositoryType, EntityVariantConfig entityVariant)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Icon = icon;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RepositoryType = repositoryType ?? throw new ArgumentNullException(nameof(repositoryType));
            EntityVariant = entityVariant ?? throw new ArgumentNullException(nameof(entityVariant));
        }

        public bool Recursive { get; set; }
        public string Alias { get; internal set; }
        internal string? Icon { get; set; }
        internal string Name { get; set; }

        internal Type RepositoryType { get; set; }

        public IEnumerable<ITreeElementConfig> CollectionsAndPages
        {
            get => _collections.Union(InlineCollections);
        }

        private IEnumerable<CollectionConfig> InlineCollections
        {
            get
            {
                var referencedCollections = new[] {
                    ListEditor?.Panes.SelectMany(x => x.SubCollectionLists.Union(x.RelatedCollectionLists)),
                    ListView?.Panes.SelectMany(x => x.SubCollectionLists.Union(x.RelatedCollectionLists)),
                    NodeEditor?.Panes.SelectMany(x => x.SubCollectionLists.Union(x.RelatedCollectionLists)),
                    NodeView?.Panes.SelectMany(x => x.SubCollectionLists.Union(x.RelatedCollectionLists)),
                };

                var referencedInlineCollections = referencedCollections
                    .Where(x => x != null)
                    .SelectMany(x => x)
                    .Where(x => x.RepositoryType != null);

                return referencedInlineCollections
                    .Select(collection => new CollectionConfig(
                        collection.CollectionAlias,
                        default,
                        $"{Name}-{collection.RepositoryType!.Name}",
                        collection.RepositoryType!,
                        new EntityVariantConfig(collection.EntityType!.Name, collection.EntityType, default))
                    {
                        ListEditor = collection.ListEditor,
                        ListView = collection.ListView
                    });
            }
        }

        internal List<EntityVariantConfig> SubEntityVariants { get; set; } = new List<EntityVariantConfig>();
        internal EntityVariantConfig EntityVariant { get; set; }

        internal List<IDataView> DataViews { get; set; } = new List<IDataView>();
        internal Type? DataViewBuilder { get; set; }

        internal TreeViewConfig? TreeView { get; set; }
        internal ListConfig? ListView { get; set; }
        internal ListConfig? ListEditor { get; set; }
        internal NodeConfig? NodeView { get; set; }
        internal NodeConfig? NodeEditor { get; set; }
    }

    internal class CollectionConfig<TEntity> : CollectionConfig, ICollectionConfig<TEntity>
        where TEntity : IEntity
    {
        internal CollectionConfig(string alias, string? icon, string name, Type repositoryType, EntityVariantConfig entityVariant)
            : base(alias, icon, name, repositoryType, entityVariant)
        {
        }

        public ICollectionConfig<TEntity> AddEntityVariant<TDerivedEntity>(string name, string icon)
            where TDerivedEntity : TEntity
        {
            SubEntityVariants.Add(new EntityVariantConfig(name, typeof(TDerivedEntity), icon));

            return this;
        }

        public ICollectionConfig<TEntity> AddDataView(string label, Expression<Func<TEntity, bool>> queryExpression)
        {
            DataViews.Add(new DataView<TEntity>(DataViews.Count, label, queryExpression));

            return this;
        }

        public ICollectionConfig<TEntity> SetDataViewBuilder<TDataViewBuilder>()
            where TDataViewBuilder : IDataViewBuilder
        {
            DataViewBuilder = typeof(TDataViewBuilder);

            return this;
        }

        public ICollectionConfig<TEntity> SetTreeView(Expression<Func<TEntity, string?>> entityNameExpression, bool showEntities = false, bool showCollections = false)
        {
            return SetTreeView(default, default, entityNameExpression, showEntities, showCollections);
        }

        public ICollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, Expression<Func<TEntity, string?>>? entityNameExpression = null, bool showEntities = false, bool showCollections = false)
        {
            return SetTreeView(entityVisibility, default, entityNameExpression, showEntities, showCollections);
        }

        public ICollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, CollectionRootVisibility rootVisibility, Expression<Func<TEntity, string?>>? entityNameExpression = null, bool showEntities = false, bool showCollections = false)
        {
            TreeView = new TreeViewConfig
            {
                EntityVisibilty = entityVisibility,
                RootVisibility = rootVisibility,
                DefaultOpenEntities = showEntities,
                DefaultOpenCollections = showCollections,
                Name = entityNameExpression == null ? null : PropertyMetadataHelper.GetExpressionMetadata(entityNameExpression) ?? throw new InvalidExpressionException(nameof(entityNameExpression))
            };

            return this;
        }

        public ICollectionConfig<TEntity> SetListView(Action<IListViewConfig<TEntity>> configure)
        {
            var config = new ListViewConfig<TEntity>();

            configure.Invoke(config);

            ListView = config;

            return this;
        }

        public ICollectionConfig<TEntity> SetListEditor(Action<IListEditorConfig<TEntity>> configure)
        {
            var config = new ListEditorConfig<TEntity>();

            configure.Invoke(config);

            ListEditor = config;

            return this;
        }

        public ICollectionConfig<TEntity> SetNodeView(Action<INodeViewConfig<TEntity>> configure)
        {
            var config = new NodeViewConfig<TEntity>();

            configure.Invoke(config);

            NodeView = config;

            return this;
        }

        public ICollectionConfig<TEntity> SetNodeEditor(Action<INodeEditorConfig<TEntity>> configure)
        {
            var config = new NodeEditorConfig<TEntity>();

            configure.Invoke(config);

            NodeEditor = config;

            return this;
        }

        public ICollectionConfig<TSubEntity> AddSubCollection<TSubEntity, TRepository>(string alias, string name, Action<ICollectionConfig<TSubEntity>> configure)
            where TSubEntity : class, IEntity
            where TRepository : IRepository
        {
            return AddSubCollection<TSubEntity, TRepository>(alias, default, name, configure);
        }

        public ICollectionConfig<TSubEntity> AddSubCollection<TSubEntity, TRepository>(string alias, string? icon, string name, Action<ICollectionConfig<TSubEntity>> configure)
            where TSubEntity : class, IEntity
            where TRepository : IRepository
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            if (alias != alias.ToUrlFriendlyString())
            {
                throw new ArgumentException($"Use lowercase, hyphened strings as alias for collections, '{alias.ToUrlFriendlyString()}' instead of '{alias}'.");
            }
            if (CmsConfig.CollectionAliases.Contains(alias))
            {
                throw new NotUniqueException(nameof(alias));
            }

            CmsConfig.CollectionAliases.Add(alias);

            var configReceiver = new CollectionConfig<TSubEntity>(
                alias,
                icon,
                name,
                typeof(TRepository),
                new EntityVariantConfig(typeof(TEntity).Name, typeof(TEntity)));

            configure.Invoke(configReceiver);

            _collections.Add(configReceiver);

            return configReceiver;
        }

        public void AddSelfAsRecursiveCollection()
        {
            var configReceiver = new CollectionConfig<TEntity>(Alias, Icon, Name, RepositoryType, EntityVariant)
            {
                Recursive = true
            };

            _collections.Add(configReceiver);
        }

        public ICollectionConfig<TEntity> ConfigureByConvention(CollectionConvention convention = CollectionConvention.ListViewNodeEditor)
        {
            //var entityType = typeof(TEntity);
            //var properties = entityType.GetProperties();
            //var usabelProperties = properties.Select(prop =>
            //{
            //    var displayAttribute = prop.GetCustomAttribute<DisplayAttribute>();
            //    if (displayAttribute != null)
            //    {
            //        return (prop, displayAttribute);
            //    }
            //    else
            //    {
            //        return default;
            //    }
            //}).Where(x => x != default);

            if (convention == CollectionConvention.ListView || convention == CollectionConvention.ListViewNodeEditor)
            {
                ListView = new ConventionListViewConfig<TEntity>(canGoToNodeEditor: convention == CollectionConvention.ListViewNodeEditor);
            }
            else if (convention == CollectionConvention.ListViewNodeEditor)
            {
                NodeEditor = new ConventionNodeEditorConfig<TEntity>();
            }
            else if (convention == CollectionConvention.ListView)
            {
                ListView = new ConventionListEditorConfig<TEntity>();
            }

            return this;
        }
    }
}
