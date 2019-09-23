using System;
using System.Linq.Expressions;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface IEditorPaneConfig<TEntity> : IHasButtons<IEditorPaneConfig<TEntity>>
        where TEntity : IEntity
    {
        IEditorFieldConfig<TEntity, TValue> AddField<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<IEditorFieldConfig<TEntity, TValue>>? configure = null);
        IEditorPaneConfig<TEntity> AddSubCollectionList<TSubEntity>(string collectionAlias, Action<SubCollectionListConfig<TSubEntity>>? configure = null)
            where TSubEntity : IEntity;
        IEditorPaneConfig<TEntity> AddRelatedCollectionList<TRelatedEntity>(string collectionAlias, Action<RelatedCollectionListConfig<TEntity, TRelatedEntity>>? configure = null)
            where TRelatedEntity : IEntity;
        IEditorPaneConfig<TEntity> SetLabel(string label);
        IEditorPaneConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate);
    }
}
