using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Interfaces.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Interfaces.Config
{
    public interface IEditorFieldConfig<TEntity, TValue> : IHasOrderBy<TEntity, IEditorFieldConfig<TEntity, TValue>>
        where TEntity : IEntity
    {
        /// <summary>
        /// Sets the name of this field, used in table and list views.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetName(string name);

        /// <summary>
        /// Sets the description of this field, displayed under the name.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        IEditorFieldConfig<TEntity, TValue> SetDescription(string description);
        
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
