using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public interface IEditorFieldConfig<TEntity> : IFieldConfig<TEntity>
        where TEntity : IEntity
    {
        IEditorFieldConfig<TEntity> SetName(string name);
        IEditorFieldConfig<TEntity> SetDescription(string description);
        IEditorFieldConfig<TEntity> SetType(EditorType type);
        IEditorFieldConfig<TEntity> SetType(Type type);
        IEditorFieldConfig<TEntity> SetReadonly(bool @readonly = true);
        IEditorFieldConfig<TEntity> SetDataCollection<TDataCollection>()
            where TDataCollection : IDataCollection;
        IEditorFieldConfig<TEntity> SetCollectionRelation<TRelatedEntity>(string collectionAlias, Action<CollectionRelationConfig<TEntity, TRelatedEntity>> configure);
        IEditorFieldConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate);
    }
}
