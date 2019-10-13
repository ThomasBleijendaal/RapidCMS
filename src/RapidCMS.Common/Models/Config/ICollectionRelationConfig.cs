using System;
using System.Linq.Expressions;

namespace RapidCMS.Common.Models.Config
{
    public interface ICollectionRelationConfig<TEntity, TRelatedEntity>
    {
        /// <summary>
        /// Sets the expression(s) that will be used as display label(s) for the relation.
        /// </summary>
        /// <param name="propertyExpressions">Expression(s) indicating how an entity should be displayed in the input / list</param>
        /// <returns></returns>
        ICollectionRelationConfig<TEntity, TRelatedEntity> SetElementDisplayProperties(params Expression<Func<TRelatedEntity, string>>[] propertyExpressions);

        /// <summary>
        /// Sets the property that will be used as id for the relation.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="propertyExpression">Expression selecting the id-property</param>
        /// <returns></returns>
        ICollectionRelationConfig<TEntity, TRelatedEntity> SetElementIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression);
        
        /// <summary>
        /// Sets the property that will be used as parentId for the related collection.
        /// </summary>
        /// <param name="propertyExpression">Expression selectiong the parentId</param>
        /// <returns></returns>
        // HACK: hardcoded IEntity.Id type (string)
        ICollectionRelationConfig<TEntity, TRelatedEntity> SetRepositoryParentIdProperty(Expression<Func<TEntity, string>> propertyExpression);
    }
}
