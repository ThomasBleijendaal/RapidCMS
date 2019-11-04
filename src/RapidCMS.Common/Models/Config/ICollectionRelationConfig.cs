using System;
using System.Linq.Expressions;
using RapidCMS.Common.Data;

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
        /// Sets the parent for the related collection.
        /// </summary>
        /// <param name="propertyExpression">Expression selectiong the parent</param>
        /// <returns></returns>
        ICollectionRelationConfig<TEntity, TRelatedEntity> SetRepositoryParent(Expression<Func<IParent, IParent?>> propertyExpression);

        /// <summary>
        /// Sets the entity that will be used as parent for the related collection.
        /// </summary>
        /// <returns></returns>
        ICollectionRelationConfig<TEntity, TRelatedEntity> SetEntityAsParent();
    }
}
