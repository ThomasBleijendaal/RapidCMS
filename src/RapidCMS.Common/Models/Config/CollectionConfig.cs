using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;

namespace RapidCMS.Common.Models.Config
{
    internal class CollectionConfig : ICollectionConfig
    {
        internal CollectionConfig(string alias, string name, EntityVariantConfig entityVariant)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EntityVariant = entityVariant ?? throw new ArgumentNullException(nameof(entityVariant));
        }

        internal bool Recursive { get; set; }
        public string Alias { get; internal set; }
        internal string Name { get; set; }

        internal Type? RepositoryType { get; set; }

        public List<ICollectionConfig> Collections { get; set; } = new List<ICollectionConfig>();
        internal List<EntityVariantConfig> SubEntityVariants { get; set; } = new List<EntityVariantConfig>();
        internal EntityVariantConfig EntityVariant { get; set; }

        internal List<IDataView> DataViews { get; set; } = new List<IDataView>();
        internal Type? DataViewBuilder { get; set; }

        internal TreeViewConfig? TreeView { get; set; }
        internal ListConfig? ListView { get; set; }
        internal ListConfig? ListEditor { get; set; }
        internal NodeConfig? NodeView { get; set; }
        internal NodeConfig? NodeEditor { get; set; }

        bool ICollectionConfig.IsUnique(string alias)
        {
            return !Collections.Any(col => col.Alias == alias);
        }
    }

    internal class CollectionConfig<TEntity> : CollectionConfig, ICollectionConfig<TEntity> 
        where TEntity : IEntity
    {
        internal CollectionConfig(string alias, string name, EntityVariantConfig entityVariant) : base(alias, name, entityVariant)
        {
        }

        public ICollectionConfig<TEntity> SetRepository<TRepository>()
           where TRepository : IRepository
        {
            RepositoryType = typeof(TRepository);

            return this;
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
            where TDataViewBuilder : DataViewBuilder<TEntity>
        {
            DataViewBuilder = typeof(TDataViewBuilder);

            return this;
        }

        public ICollectionConfig<TEntity> SetTreeView(Expression<Func<TEntity, string>> entityNameExpression)
        {
            return SetTreeView(default, default, entityNameExpression);
        }

        public ICollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, Expression<Func<TEntity, string>>? entityNameExpression = null)
        {
            return SetTreeView(entityVisibility, default, entityNameExpression);
        }

        public ICollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, CollectionRootVisibility rootVisibility, Expression<Func<TEntity, string>>? entityNameExpression)
        {
            TreeView = new TreeViewConfig
            {
                EntityVisibilty = entityVisibility,
                RootVisibility = rootVisibility,
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
            return SetListEditor(default, default, configure);
        }

        public ICollectionConfig<TEntity> SetListEditor(ListType listEditorType, Action<IListEditorConfig<TEntity>> configure)
        {
            return SetListEditor(listEditorType, default, configure);
        }

        public ICollectionConfig<TEntity> SetListEditor(ListType listEditorType, EmptyVariantColumnVisibility emptyVariantColumnVisibility, Action<IListEditorConfig<TEntity>> configure)
        {
            var config = new ListEditorConfig<TEntity>();

            configure.Invoke(config);

            config.ListEditorType = listEditorType;
            config.EmptyVariantColumnVisibility = emptyVariantColumnVisibility;

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
    }
}
