using System;
using System.Linq;
using System.Linq.Expressions;

namespace RapidCMS.Common.Data
{
    public interface IQuery
    {
        int Skip { get; }
        int Take { get; }
        string? SearchTerm { get; }

        void HasMoreData(bool hasMoreData);

        bool MoreDataAvailable { get; }
    }

    public interface IQuery<TEntity> : IQuery
    {
        /// <summary>
        /// Expression corresponding to the selected data view. Can be directly inserted into IQueryable.Where.
        /// </summary>
        Expression<Func<TEntity, bool>>? DataViewExpression { get; }

        /// <summary>
        /// Method that applies the effective ordering specified by the user. 
        /// </summary>
        /// <param name="queryable">Queryable that will be returned with additional OrderBy(Descending)s and ThenBy(Descending)s.</param>
        /// <returns></returns>
        IQueryable<TEntity> ApplyOrder(IQueryable<TEntity> queryable);
    }
}
