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
    public class CollectionConfig : ICollectionRoot
    {
        internal bool Recursive { get; set; }
        internal string Alias { get; set; }
        internal string Name { get; set; }

        internal Type RepositoryType { get; set; }

        public List<CollectionConfig> Collections { get; set; } = new List<CollectionConfig>();
        internal List<EntityVariantConfig> SubEntityVariants { get; set; } = new List<EntityVariantConfig>();
        internal EntityVariantConfig EntityVariant { get; set; }

        internal List<IDataView> DataViews { get; set; } = new List<IDataView>();
        internal Type? DataViewBuilder { get; set; }

        internal TreeViewConfig TreeView { get; set; }
        internal ListConfig ListView { get; set; }
        internal ListConfig ListEditor { get; set; }
        internal NodeConfig NodeView { get; set; }
        internal NodeConfig NodeEditor { get; set; }

        public bool IsUnique(string alias)
        {
            return !Collections.Any(col => col.Alias == alias);
        }
    }

    public class CollectionConfig<TEntity> : CollectionConfig
        where TEntity : IEntity
    {
        public CollectionConfig<TEntity> SetRepository<TRepository>()
           where TRepository : IRepository
        {
            RepositoryType = typeof(TRepository);

            return this;
        }

        public CollectionConfig<TEntity> AddEntityVariant<TDerivedEntity>(string name, string icon)
            where TDerivedEntity : TEntity
        {
            SubEntityVariants.Add(new EntityVariantConfig(name, typeof(TDerivedEntity), icon));

            return this;
        }

        public CollectionConfig<TEntity> AddDataView(string label, Expression<Func<TEntity, bool>> queryExpression)
        {
            DataViews.Add(new DataView<TEntity>(DataViews.Count, label, queryExpression));

            return this;
        }

        public CollectionConfig<TEntity> SetDataViewBuilder<TDataViewBuilder>()
            where TDataViewBuilder : DataViewBuilder<TEntity>
        {
            DataViewBuilder = typeof(TDataViewBuilder);

            return this;
        }

        public CollectionConfig<TEntity> SetTreeView(Expression<Func<TEntity, string>> nameExpression)
        {
            return SetTreeView(default, default, nameExpression);
        }

        public CollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, Expression<Func<TEntity, string>> nameExpression)
        {
            return SetTreeView(entityVisibility, default, nameExpression);
        }

        public CollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, CollectionRootVisibility rootVisibility, Expression<Func<TEntity, string>>? nameExpression)
        {
            TreeView = new TreeViewConfig
            {
                EntityVisibilty = entityVisibility,
                RootVisibility = rootVisibility,
                Name = nameExpression == null ? null : PropertyMetadataHelper.GetExpressionMetadata(nameExpression) ?? throw new InvalidExpressionException(nameof(nameExpression))
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

        public CollectionConfig<TEntity> SetListEditor(Action<ListEditorConfig<TEntity>> configure)
        {
            return SetListEditor(default, default, configure);
        }

        public CollectionConfig<TEntity> SetListEditor(ListType listEditorType, Action<ListEditorConfig<TEntity>> configure)
        {
            return SetListEditor(listEditorType, default, configure);
        }

        public CollectionConfig<TEntity> SetListEditor(ListType listEditorType, EmptyVariantColumnVisibility emptyVariantColumnVisibility, Action<ListEditorConfig<TEntity>> configure)
        {
            var config = new ListEditorConfig<TEntity>();

            configure.Invoke(config);

            config.ListEditorType = listEditorType;
            config.EmptyVariantColumnVisibility = emptyVariantColumnVisibility;

            ListEditor = config;

            return this;
        }

        public CollectionConfig<TEntity> SetNodeView(Action<NodeViewConfig<TEntity>> configure)
        {
            var config = new NodeViewConfig<TEntity>();

            configure.Invoke(config);

            NodeView = config;

            return this;
        }

        public CollectionConfig<TEntity> SetNodeEditor(Action<NodeEditorConfig<TEntity>> configure)
        {
            var config = new NodeEditorConfig<TEntity>();

            configure.Invoke(config);

            NodeEditor = config;

            return this;
        }
    }
}
