using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
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

        internal CollectionConfig(string alias, string? parentAlias, string? icon, string? color, string name, Type repositoryType, EntityVariantConfig entityVariant)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            ParentAlias = parentAlias;
            Icon = icon;
            Color = color;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RepositoryType = repositoryType ?? throw new ArgumentNullException(nameof(repositoryType));
            EntityVariant = entityVariant ?? throw new ArgumentNullException(nameof(entityVariant));
        }

        public string Alias { get; internal set; }
        internal string? Icon { get; set; }
        internal string? Color { get; set; }
        public string Name { get; set; }

        public Type RepositoryType { get; set; }

        public string? ParentAlias { get; set; }

        public IEnumerable<ITreeElementConfig> CollectionsAndPages
        {
            get => _collections.Union(InlineCollections);
        }

        public IEnumerable<Type> RepositoryTypes
        {
            get => new[] { RepositoryType }.Union(_collections.Union(InlineCollections).SelectMany(x => x.RepositoryTypes));
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
                    .SelectMany(x => x!)
                    .Where(x => x.RepositoryType != null);

                return referencedInlineCollections
                    .Select(collection => new CollectionConfig(
                        collection.CollectionAlias,
                        Alias,
                        default,
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
        internal ElementConfig? ElementConfig { get; set; }
        internal ListConfig? ListView { get; set; }
        internal ListConfig? ListEditor { get; set; }
        internal NodeConfig? NodeView { get; set; }
        internal NodeConfig? NodeEditor { get; set; }
    }

    internal class CollectionConfig<TEntity> : CollectionConfig, ICollectionConfig<TEntity>
        where TEntity : IEntity
    {
        internal CollectionConfig(string alias, string? parentAlias, string? icon, string? color, string name, Type repositoryType, EntityVariantConfig entityVariant)
            : base(alias, parentAlias, icon, color, name, repositoryType, entityVariant)
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

            var idExpression = PropertyMetadataHelper.GetPropertyMetadata<IEntity, string?>(x => x.Id) ?? throw new InvalidOperationException("IEntity.Id expression failed");
            ElementConfig ??= new ElementConfig(idExpression, TreeView.Name ?? (IExpressionMetadata)idExpression);

            return this;
        }

        public ICollectionConfig<TEntity> SetElementConfiguration<TIdValue>(Expression<Func<TEntity, TIdValue>> elementIdExpression, params Expression<Func<TEntity, string?>>[] elementDisplayExpressions)
        {
            ElementConfig = new ElementConfig(
                PropertyMetadataHelper.GetPropertyMetadata(elementIdExpression) ?? throw new InvalidOperationException("Cannot convert elementIdExpression to IExpressionMetadata"),
                elementDisplayExpressions.Select(PropertyMetadataHelper.GetExpressionMetadata).OfType<IExpressionMetadata>().ToArray());

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

        public ICollectionConfig<TEntity> AddSubCollection(string alias)
        {
            var configReceiver = new ReferencedCollectionConfig(alias);

            _collections.Add(configReceiver);

            return this;
        }

        public ICollectionConfig<TSubEntity> AddSubCollection<TSubEntity, TRepository>(string alias, string name, Action<ICollectionConfig<TSubEntity>> configure)
            where TSubEntity : class, IEntity
            where TRepository : IRepository 
            => AddSubCollection<TSubEntity, TRepository>(alias, default, default, name, configure);

        public ICollectionConfig<TSubEntity> AddSubCollection<TSubEntity, TRepository>(string alias, string? icon, string name, Action<ICollectionConfig<TSubEntity>> configure)
            where TSubEntity : class, IEntity
            where TRepository : IRepository 
            => AddSubCollection<TSubEntity, TRepository>(alias, icon, default, name, configure);

        public ICollectionConfig<TSubEntity> AddSubCollection<TSubEntity, TRepository>(string alias, string? icon, string? color, string name, Action<ICollectionConfig<TSubEntity>> configure)
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
                Alias,
                icon,
                color,
                name,
                typeof(TRepository),
                new EntityVariantConfig(typeof(TSubEntity).Name, typeof(TSubEntity)));

            configure.Invoke(configReceiver);

            _collections.Add(configReceiver);

            return configReceiver;
        }

        public ICollectionConfig<TEntity> AddSelfAsRecursiveCollection()
        {
            var configReceiver = new ReferencedCollectionConfig(Alias);

            ParentAlias = Alias;

            _collections.Add(configReceiver);

            return this;
        }

        public ICollectionDetailPageEditorConfig<TDetailEntity> AddDetailPage<TDetailEntity, TDetailRepository>(string alias, string name, Action<ICollectionDetailPageEditorConfig<TDetailEntity>> configure)
            where TDetailEntity : IEntity
            where TDetailRepository : IRepository
            => AddDetailPage<TDetailEntity, TDetailRepository>(alias, default, default, name, configure);

        public ICollectionDetailPageEditorConfig<TDetailEntity> AddDetailPage<TDetailEntity, TDetailRepository>(string alias, string? icon, string name, Action<ICollectionDetailPageEditorConfig<TDetailEntity>> configure)
            where TDetailEntity : IEntity
            where TDetailRepository : IRepository
            => AddDetailPage<TDetailEntity, TDetailRepository>(alias, icon, default, name, configure);

        public ICollectionDetailPageEditorConfig<TDetailEntity> AddDetailPage<TDetailEntity, TDetailRepository>(string alias, string? icon, string? color, string name, Action<ICollectionDetailPageEditorConfig<TDetailEntity>> configure)
            where TDetailEntity : IEntity
            where TDetailRepository : IRepository
        {
            var configReceiver = new DetailPageConfig<TDetailEntity>(alias, Alias, icon, color, name, typeof(TDetailRepository), new EntityVariantConfig(typeof(TDetailEntity).Name, typeof(TDetailEntity)));

            configure?.Invoke(configReceiver);

            _collections.Add(configReceiver);

            return configReceiver;
        }

        public ICollectionConfig<TEntity> ConfigureByConvention(CollectionConvention convention = CollectionConvention.ListViewNodeEditor)
        {
            if (convention == CollectionConvention.ListView ||
                convention == CollectionConvention.ListViewNodeView ||
                convention == CollectionConvention.ListViewNodeEditor)
            {
                ListView = new ConventionListViewConfig<TEntity>(
                    canGoToNodeEditor: convention == CollectionConvention.ListViewNodeEditor,
                    canGoToNodeView: convention == CollectionConvention.ListViewNodeView);
            }

            if (convention == CollectionConvention.ListEditor || convention == CollectionConvention.ListBlockEditor)
            {
                ListEditor = new ConventionListEditorConfig<TEntity>(convention == CollectionConvention.ListBlockEditor ? ListType.Block : ListType.Table);
            }

            if (convention == CollectionConvention.ListViewNodeView)
            {
                NodeView = new ConventionNodeViewConfig<TEntity>();
            }

            if (convention == CollectionConvention.ListViewNodeEditor)
            {
                NodeEditor = new ConventionNodeEditorConfig<TEntity>();
            }

            return this;
        }
    }
}
