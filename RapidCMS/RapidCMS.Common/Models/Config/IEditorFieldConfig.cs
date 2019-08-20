using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public interface IEditorFieldConfig<TEntity, TValue> : IFieldConfig<TEntity>
        where TEntity : IEntity
    {
        IEditorFieldConfig<TEntity, TValue> SetName(string name);
        IEditorFieldConfig<TEntity, TValue> SetDescription(string description);
        IEditorFieldConfig<TEntity, TValue> SetType(EditorType type);
        IEditorFieldConfig<TEntity, TValue> SetType(Type type);
        IEditorFieldConfig<TEntity, TValue> SetReadonly(bool @readonly = true);
        IEditorFieldConfig<TEntity, TValue> SetDataCollection<TDataCollection>()
            where TDataCollection : IDataCollection;
        IEditorFieldConfig<TEntity, TValue> SetCollectionRelation<TRelatedEntity>(
            string collectionAlias,
            Action<CollectionRelationConfig<TEntity, TRelatedEntity>> configure);
        IEditorFieldConfig<TEntity, TValue> SetCollectionRelation<TRelatedEntity, TKey>(
            Expression<Func<TValue, IEnumerable<TKey>>> relatedElements,
            string collectionAlias, 
            Action<CollectionRelationConfig<TEntity, TRelatedEntity>> configure);
        IEditorFieldConfig<TEntity, TValue> VisibleWhen(Func<TEntity, bool> predicate);
    }
}
