using System;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public interface ICollectionConfig<TEntity> : ICollectionConfig
        where TEntity : IEntity
    {
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
            where TDataViewBuilder : DataViewBuilder<TEntity>;
        
        /// <summary>
        /// Sets the ListEditor of this collection
        /// </summary>
        /// <param name="configure">Action used to configure the ListEditor</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetListEditor(Action<IListEditorConfig<TEntity>> configure);
        
        /// <summary>
        /// Sets the ListEditor of this collection
        /// </summary>
        /// <param name="listEditorType">Controls how the ListEditor is displayed</param>
        /// <param name="configure">Action used to configure the ListEditor</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetListEditor(ListType listEditorType, Action<IListEditorConfig<TEntity>> configure);
        
        /// <summary>
        /// Sets the ListEditor of this collection
        /// </summary>
        /// <param name="listEditorType">Controls how the ListEditor is displayed</param>
        /// <param name="emptyVariantColumnVisibility">Controls whether empty columns in the table should be collapsed. Only required when the
        /// collection uses multiple EntityVariants, with seperate sets of properties which are not shared between the variants. Collapsing
        /// the empty cell will reduce the number of columns required, and makes the table more readable.</param>
        /// <param name="configure">Action used to configure the ListEditor</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetListEditor(ListType listEditorType, EmptyVariantColumnVisibility emptyVariantColumnVisibility, Action<IListEditorConfig<TEntity>> configure);
        
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
        /// Sets the given class as repository of this collection.
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetRepository<TRepository>();
        
        /// <summary>
        /// Sets how the collection should be displayed in the tree.
        /// </summary>
        /// <param name="entityVisibility">Controls whether the entities of this collection are visible in the tree</param>
        /// <param name="entityNameExpression">Expression used to display entities of this collection</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, Expression<Func<TEntity, string>>? entityNameExpression = null);
        
        /// <summary>
        /// Sets how the collection should be displayed in the tree.
        /// </summary>
        /// <param name="entityVisibility">Controls whether the entities of this collection are visible in the tree</param>
        /// <param name="rootVisibility">Controls whether the root of this collection is visible in the tree</param>
        /// <param name="entityNameExpression">Expression used to display entities of this collection</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, CollectionRootVisibility rootVisibility, Expression<Func<TEntity, string>>? entityNameExpression);
        
        /// <summary>
        /// Sets how the collection should be displayed in the tree.
        /// </summary>
        /// <param name="entityNameExpression">Expression used to display entities of this collection</param>
        /// <returns></returns>
        ICollectionConfig<TEntity> SetTreeView(Expression<Func<TEntity, string>> entityNameExpression);
    }
}
