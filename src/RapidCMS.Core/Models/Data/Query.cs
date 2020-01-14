using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data
{
    public sealed class Query : IQuery
    {
        internal LambdaExpression? QueryExpression;
        internal IEnumerable<IOrderBy>? OrderByExpressions;

        public static Query Default()
        {
            return new Query
            {
                Skip = 0,
                Take = 1000
            };
        }

        public static Query Create(int pageSize, int pageNumber, string? searchTerm, int? activeTab)
        {
            return new Query
            {
                Skip = pageSize * (pageNumber - 1),
                Take = pageSize,
                SearchTerm = searchTerm,
                ActiveTab = activeTab
            };
        }

        public void HasMoreData(bool hasMoreData)
        {
            MoreDataAvailable = hasMoreData;
        }

        public void SetDataViewExpression(LambdaExpression expression)
        {
            QueryExpression = expression;
        }

        public void SetOrderByExpressions(IEnumerable<IOrderBy> expressions)
        {
            OrderByExpressions = expressions;
        }

        public int? ActiveTab { get; private set; }

        public int Skip { get; private set; }
        public int Take { get; private set; }

        public string? SearchTerm { get; private set; }

        public bool MoreDataAvailable { get; private set; } = false;
    }
}
