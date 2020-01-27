using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Abstractions.Data
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
        /// Gets the active dataview selected by the user. Use ApplyDataView to apply the QueryExpression to your IQueryable.
        /// 
        /// Use this property to determine what the active data view Id is, when your respository does not support IQueryable.
        /// </summary>
        IDataView? ActiveDataView { get; }

        /// <summary>
        /// Gets the active order by instructions selected by the user. Use ApplyOrder to apply the ordering to your IQuerable.
        /// 
        /// Use this property to determine what the active order by instructions are, when your repository does not support IQueryable.
        /// </summary>
        IEnumerable<IOrderBy> ActiveOrderBys { get; }

        /// <summary>
        /// Method that applies the effective data view selected by the user. 
        /// </summary>
        /// <param name="queryable">Queryable that will be returned with additional Where specified in DataViewBuilder.</param>
        /// <returns></returns>
        IQueryable<TEntity> ApplyDataView(IQueryable<TEntity> queryable);

        /// <summary>
        /// Method that applies the effective ordering specified by the user. 
        /// </summary>
        /// <param name="queryable">Queryable that will be returned with additional OrderBy(Descending)s and ThenBy(Descending)s.</param>
        /// <returns></returns>
        IQueryable<TEntity> ApplyOrder(IQueryable<TEntity> queryable);
    }
}
