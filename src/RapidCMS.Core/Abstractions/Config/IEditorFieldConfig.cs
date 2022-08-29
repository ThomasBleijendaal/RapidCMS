using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IEditorFieldConfig<TEntity, TValue>
        : IHasOrderBy<TEntity, IEditorFieldConfig<TEntity, TValue>>,
        IHasNameDescription<IEditorFieldConfig<TEntity, TValue>>,
        IHasPlaceholder<IEditorFieldConfig<TEntity, TValue>>,
        IHasConfigurability<TEntity, IEditorFieldConfig<TEntity, TValue>>
        where TEntity : IEntity
    {
        /// <summary>
        /// Sets the type of build-in editor used for this field.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetType(EditorType type);

        /// <summary>
        /// Sets the type of build-in display used for this field.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetType(DisplayType type);

        /// <summary>
        /// Sets the type of custom razor component used for this field.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetType(Type type);

        /// <summary>
        /// Binds a DataCollection to this field. This data collection is used by dropdowns and selects to display options.
        /// </summary>
        /// <typeparam name="TDataCollection"></typeparam>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetDataCollection<TDataCollection>()
            where TDataCollection : IDataCollection;

        /// <summary>
        /// Binds a DataCollection to this field. This data collection is used by dropdowns and selects to display options.
        /// </summary>
        /// <typeparam name="TDataCollection"></typeparam>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetDataCollection<TDataCollection>(TDataCollection dataCollection)
            where TDataCollection : IDataCollection;

        /// <summary>
        /// Binds a DataCollection to this field. This data collection is used by dropdowns and selects to display options.
        /// 
        /// The TConfig object is passed into the data collection.
        /// </summary>
        /// <typeparam name="TDataCollection"></typeparam>
        /// <typeparam name="TConfig"></typeparam>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetDataCollection<TDataCollection, TConfig>(TConfig config)
            where TDataCollection : IDataCollection;

        /// <summary>
        /// Binds a Collection to this field. This collection is used by dropdowns and selects to display options.
        /// 
        /// Uses the default Element configuration of the referenced collection to display the options.
        /// </summary>
        /// <param name="collectionAlias">Alias of the collection to bind.</param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetCollectionRelation(string collectionAlias);

        /// <summary>
        /// Binds a Collection to this field. This collection is used by dropdowns and selects to display options.
        /// </summary>
        /// <typeparam name="TRelatedEntity">Entity of the bound collection.</typeparam>
        /// <param name="collectionAlias">Alias of the collection to bind.</param>
        /// <param name="configure">Action to configure relation.</param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetCollectionRelation<TRelatedEntity>(
            string collectionAlias,
            Action<ICollectionRelationConfig<TEntity, TRelatedEntity>> configure);

        /// <summary>
        /// Binds a Collection to this field. This collection is used by dropdowns and selects to display options.
        /// </summary>
        /// <typeparam name="TRelatedEntity">Entity of the bound collection.</typeparam>
        /// <typeparam name="TRelatedRepository">Type of the repository.</typeparam>
        /// <param name="configure">Action to configure relation.</param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetCollectionRelation<TRelatedEntity, TRelatedRepository>(
            Action<ICollectionRelationConfig<TEntity, TRelatedEntity>> configure)
            where TRelatedRepository : IRepository;

        /// <summary>
        /// Binds a Collection to this field. This collection is used by dropdowns and selects to display options.
        /// 
        /// Use this overload to bind a collection for a many-to-many relation in EF Core backed repositories:
        /// - Use the joining table as property for this editor.
        /// - Use the relatedElements expression as selector for selected elements to display in the EditorType.MultiSelect or any other RelationAttribute(RelationType.Many) editor.
        /// </summary>
        /// <typeparam name="TRelatedEntity">Entity of the bound collection.</typeparam>
        /// <typeparam name="TKey">Type of the foreign key</typeparam>
        /// <param name="relatedElements">Expression for selected entities.</param>
        /// <param name="collectionAlias">Alias of the collection to bind.</param>
        /// <param name="configure">Action to configure relation.</param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetCollectionRelation<TRelatedEntity, TKey>(
            Expression<Func<TValue, IEnumerable<TKey>>> relatedElements,
            string collectionAlias,
            Action<ICollectionRelationConfig<TEntity, TRelatedEntity>> configure);

        /// <summary>
        /// Binds a Collection to this field. This collection is used by dropdowns and selects to display options.
        /// 
        /// Use this overload to bind a collection for a many-to-many relation in EF Core backed repositories:
        /// - Use the joining table as property for this editor.
        /// - Use the relatedElements expression as selector for selected elements to display in the EditorType.MultiSelect or any other RelationAttribute(RelationType.Many) editor.
        /// </summary>
        /// <typeparam name="TRelatedEntity">Entity of the bound collection.</typeparam>
        /// <typeparam name="TKey">Type of the foreign key</typeparam>
        /// <typeparam name="TRelatedRepository">Type of the related repository</typeparam>
        /// <param name="relatedElements">Expression for selected entities.</param>
        /// <param name="configure">Action to configure relation.</param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetCollectionRelation<TRelatedEntity, TKey, TRelatedRepository>(
            Expression<Func<TValue, IEnumerable<TKey>>> relatedElements,
            Action<ICollectionRelationConfig<TEntity, TRelatedEntity>> configure)
            where TRelatedRepository : IRepository;

        /// <summary>
        /// Sets an expression which determines whether this field should be visible.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> VisibleWhen(Func<TEntity, EntityState, bool> predicate);

        /// <summary>
        /// Sets an expression which determine whether this field should be disabled.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> DisableWhen(Func<TEntity, EntityState, bool> predicate);
    }
}
