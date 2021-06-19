using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface ICollectionConfig : ITreeElementConfig
    {
        string? ParentAlias { get; }
        IEnumerable<ITreeElementConfig> CollectionsAndPages { get; }

        Type RepositoryType { get; }
        IEnumerable<Type> RepositoryTypes { get; }
    }

    public interface ICollectionConfig<TEntity> : ICollectionConfig
        where TEntity : IEntity
    {
        /// <summary>
        /// Adds the given collection as sub collection to the current collection.
        /// 
        /// Because it's referenced only by alias, no configuration can be added to this sub collection.
        /// 
        /// NOTE: The reference between this sub collection and it's parent collection is weak and the sub collection will not be able to determine it is nested here. (see #101)
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        ICollectionConfig<TEntity> AddSubCollection(string alias);

        /// <summary>
        /// Adds a sub collection to the current collection.
        /// </summary>
        /// <typeparam name="TSubEntity">Type of the entity of this sub collection</typeparam>
        /// <typeparam name="TRepository">Type of the repository this sub collection will use</typeparam>
        /// <param name="alias">Alias of the sub collection</param>
        /// <param name="name">Human readable name of this sub collection</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICollectionConfig<TSubEntity> AddSubCollection<TSubEntity, TRepository>(string alias, string name, Action<ICollectionConfig<TSubEntity>> configure)
            where TSubEntity : class, IEntity
            where TRepository : IRepository;

        /// <summary>
        /// Adds a sub collection to the current collection.
        /// </summary>
        /// <typeparam name="TSubEntity">Type of the entity of this sub collection</typeparam>
        /// <typeparam name="TRepository">Type of the repository this sub collection will use</typeparam>
        /// <param name="alias">Alias of the sub collection</param>
        /// <param name="icon">Icon for this sub collection (https://developer.microsoft.com/en-us/fluentui#/styles/web/icons)</param>
        /// <param name="name">Human readable name of this sub collection</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICollectionConfig<TSubEntity> AddSubCollection<TSubEntity, TRepository>(string alias, string? icon, string name, Action<ICollectionConfig<TSubEntity>> configure)
            where TSubEntity : class, IEntity
            where TRepository : IRepository;

        /// <summary>
        /// Adds a sub collection to the current collection.
        /// </summary>
        /// <typeparam name="TSubEntity">Type of the entity of this sub collection</typeparam>
        /// <typeparam name="TRepository">Type of the repository this sub collection will use</typeparam>
        /// <param name="alias">Alias of the sub collection</param>
        /// <param name="icon">Icon for this sub collection (https://developer.microsoft.com/en-us/fluentui#/styles/web/icons)</param>
        /// <param name="color">The color of this sub collection (https://developer.microsoft.com/en-us/fluentui#/styles/web/colors/personas)</param>
        /// <param name="name">Human readable name of this sub collection</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICollectionConfig<TSubEntity> AddSubCollection<TSubEntity, TRepository>(string alias, string? icon, string? color, string name, Action<ICollectionConfig<TSubEntity>> configure)
            where TSubEntity : class, IEntity
            where TRepository : IRepository;

        /// <summary>
        /// Adds itself as sub collection.
        /// 
        /// Because this is a self-reference, no configuration can be added to this sub collection.
        /// </summary>
        ICollectionConfig<TEntity> AddSelfAsRecursiveCollection();

        /// <summary>
        /// Add a detail page to the current collection. A detail page is a NodeEditor with its own entity type and repository, and allow
        /// for creating specialized editors.
        /// </summary>
        /// <typeparam name="TDetailEntity"></typeparam>
        /// <typeparam name="TDetailRepository"></typeparam>
        /// <param name="alias">Alias of the detail page</param>
        /// <param name="name">Human readable name of this detail page</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICollectionDetailPageEditorConfig<TDetailEntity> AddDetailPage<TDetailEntity, TDetailRepository>(string alias, string name, Action<ICollectionDetailPageEditorConfig<TDetailEntity>> configure)
            where TDetailEntity : IEntity
            where TDetailRepository : IRepository;

        /// <summary>
        /// Add a detail page to the current collection. A detail page is a NodeEditor with its own entity type and repository, and allow
        /// for creating specialized editors.
        /// </summary>
        /// <typeparam name="TDetailEntity"></typeparam>
        /// <typeparam name="TDetailRepository"></typeparam>
        /// <param name="alias">Alias of the detail page</param>
        /// <param name="icon">Icon for this sub collection (https://developer.microsoft.com/en-us/fluentui#/styles/web/icons)</param>
        /// <param name="name">Human readable name of this detail page</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICollectionDetailPageEditorConfig<TDetailEntity> AddDetailPage<TDetailEntity, TDetailRepository>(string alias, string? icon, string name, Action<ICollectionDetailPageEditorConfig<TDetailEntity>> configure)
            where TDetailEntity : IEntity
            where TDetailRepository : IRepository;

        /// <summary>
        /// Add a detail page to the current collection. A detail page is a NodeEditor with its own entity type and repository, and allow
        /// for creating specialized editors.
        /// </summary>
        /// <typeparam name="TDetailEntity"></typeparam>
        /// <typeparam name="TDetailRepository"></typeparam>
        /// <param name="alias">Alias of the detail page</param>
        /// <param name="icon">Icon for this sub collection (https://developer.microsoft.com/en-us/fluentui#/styles/web/icons)</param>
        /// <param name="color">The color of this sub collection (https://developer.microsoft.com/en-us/fluentui#/styles/web/colors/personas)</param>
        /// <param name="name">Human readable name of this detail page</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICollectionDetailPageEditorConfig<TDetailEntity> AddDetailPage<TDetailEntity, TDetailRepository>(string alias, string? icon, string? color, string name, Action<ICollectionDetailPageEditorConfig<TDetailEntity>> configure)
            where TDetailEntity : IEntity
            where TDetailRepository : IRepository;

        /// <summary>
        /// Adds a data view to the collection. Data views are displayed as seperate tabs on the collection, and allow
        /// the user to filter the collection data easily.
        /// </summary>
        /// <param name="label">Human readable label of this data view</param>
        /// <param name="queryExpression">Query defining this data view</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> AddDataView(string label, Expression<Func<TEntity, bool>> queryExpression);

        /// <summary>
        /// Adds an entity variant to the collection. Entity variants are derivatives of TEntity.
        /// </summary>
        /// <typeparam name="TDerivedEntity"></typeparam>
        /// <param name="name">Human readable name of this variant</param>
        /// <param name="icon">Name of ion icon. (https://developer.microsoft.com/en-us/fluentui#/styles/web/icons)</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> AddEntityVariant<TDerivedEntity>(string name, string icon)
            where TDerivedEntity : TEntity;

        /// <summary>
        /// Adds a data view builder to the collection. Data view builders allow for creating dynamic data views.
        /// </summary>
        /// <typeparam name="TDataViewBuilder"></typeparam>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetDataViewBuilder<TDataViewBuilder>()
            where TDataViewBuilder : IDataViewBuilder;

        /// <summary>
        /// Sets the ListEditor of this collection
        /// </summary>
        /// <param name="configure">Action used to configure the ListEditor</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetListEditor(Action<IListEditorConfig<TEntity>> configure);

        /// <summary>
        /// Sets the ListView of this collection
        /// </summary>
        /// <param name="configure">Action used to configure the ListView</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetListView(Action<IListViewConfig<TEntity>> configure);

        /// <summary>
        /// Sets the NodeEditor of this collection
        /// </summary>
        /// <param name="configure">Action used to configure the NodeEditor</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetNodeEditor(Action<INodeEditorConfig<TEntity>> configure);

        /// <summary>
        /// Sets the NodeView of this collection
        /// </summary>
        /// <param name="configure">Action used to configure the NodeView</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetNodeView(Action<INodeViewConfig<TEntity>> configure);

        /// <summary>
        /// Sets the UI of this collection by convention
        /// 
        /// The resulting UI will be based on what DisplayAttribute
        /// s on primitive properties are configured on the model.
        /// The Name and Description property of 
        /// </summary>
        /// <param name="convention">Type of convention to use.</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> ConfigureByConvention(CollectionConvention convention = CollectionConvention.ListViewNodeEditor);

        /// <summary>
        /// Sets how the collection should be displayed in the tree.
        /// </summary>
        /// <param name="entityVisibility">Controls whether the entities of this collection are visible in the tree</param>
        /// <param name="entityNameExpression">Expression used to display entities of this collection</param>
        /// <param name="showEntitiesOnStartup">When set to true, the tree will open the collection open on default.</param>
        /// <param name="showCollectionsOnStartup">When set to true, the tree will open the entities open on default.</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, Expression<Func<TEntity, string?>>? entityNameExpression = null, bool showEntitiesOnStartup = false, bool showCollectionsOnStartup = false);

        /// <summary>
        /// Sets how the collection should be displayed in the tree.
        /// </summary>
        /// <param name="entityVisibility">Controls whether the entities of this collection are visible in the tree</param>
        /// <param name="rootVisibility">Controls whether the root of this collection is visible in the tree</param>
        /// <param name="entityNameExpression">Expression used to display entities of this collection</param>
        /// <param name="showEntitiesOnStartup">When set to true, the tree will open the collection open on default.</param>
        /// <param name="showCollectionsOnStartup">When set to true, the tree will open the entities open on default.</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, CollectionRootVisibility rootVisibility, Expression<Func<TEntity, string?>>? entityNameExpression = null, bool showEntitiesOnStartup = false, bool showCollectionsOnStartup = false);

        /// <summary>
        /// Sets how the collection should be displayed in the tree.
        /// </summary>
        /// <param name="entityNameExpression">Expression used to display entities of this collection</param>
        /// <param name="showEntitiesOnStartup">When set to true, the tree will open the collection open on default.</param>
        /// <param name="showCollectionsOnStartup">When set to true, the tree will open the entities open on default.</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetTreeView(Expression<Func<TEntity, string?>> entityNameExpression, bool showEntitiesOnStartup = false, bool showCollectionsOnStartup = false);

        /// <summary>
        /// Sets how elements in dropdowns and entity pickers should be displayed when explicit configuration is not available.
        /// </summary>
        /// <param name="elementIdExpression">Expression used as Id for elements</param>
        /// <param name="elementDisplayExpressions">Expressions used as Display labels from elements</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetElementConfiguration<TIdValue>(
            Expression<Func<TEntity, TIdValue>> elementIdExpression,
            params Expression<Func<TEntity, string?>>[] elementDisplayExpressions);
    }
}
