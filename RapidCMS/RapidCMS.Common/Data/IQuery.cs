using System;
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
        where TEntity : IEntity
    {
        Expression<Func<TEntity, bool>>? DataViewExpression { get; }
    }
}
