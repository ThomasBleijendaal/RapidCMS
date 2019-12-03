using System;
using System.Linq.Expressions;

namespace RapidCMS.Common.Data
{
    internal class TypedQuery<TEntity> : IQuery, IQuery<TEntity>
    {
        private readonly Query _query;

        public static IQuery<TEntity> Convert(IQuery query)
        {
            return query switch
            {
                Query q => new TypedQuery<TEntity>(q),
                TypedQuery<TEntity> q => q,
                _ => throw new InvalidOperationException()
            }; 
        }

        public TypedQuery(Query query)
        {
            _query = query;
        }

        public int Skip => _query.Skip;

        public int Take => _query.Take;

        public string? SearchTerm => _query.SearchTerm;

        public bool MoreDataAvailable => _query.MoreDataAvailable;

        public Expression<Func<TEntity, bool>>? DataViewExpression => _query.QueryExpression as Expression<Func<TEntity, bool>>;

        public void HasMoreData(bool hasMoreData)
        {
            _query.HasMoreData(hasMoreData);
        }
    }
}
