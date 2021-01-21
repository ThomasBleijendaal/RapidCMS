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

        bool Recursive { get; }
    }

    public interface ICollectionConfig<TEntity> : ICollectionConfig
        where TEntity : IEntity
    {
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
        /// </summary>
        void AddSelfAsRecursiveCollection();

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
        /// <param name="icon">Name of ion icon.</param>
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
    }
}
