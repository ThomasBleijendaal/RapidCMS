using System;
using System.Linq.Expressions;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface IDisplayPaneConfig<TEntity> : IHasButtons<IDisplayPaneConfig<TEntity>>
        where TEntity : IEntity
    {
        IDisplayFieldConfig<TEntity, string> AddField(Expression<Func<TEntity, string>> displayExpression, Action<IDisplayFieldConfig<TEntity, string>>? configure = null);
        IDisplayPaneConfig<TEntity> AddSubCollectionList<TSubEntity>(string collectionAlias, Action<SubCollectionListConfig<TSubEntity>>? configure = null)
            where TSubEntity : IEntity;
        IDisplayPaneConfig<TEntity> AddRelatedCollectionList<TRelatedEntity>(string collectionAlias, Action<RelatedCollectionListConfig<TEntity, TRelatedEntity>>? configure = null)
            where TRelatedEntity : IEntity;
        IDisplayPaneConfig<TEntity> SetLabel(string label);
        IDisplayPaneConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate);
    }
}
