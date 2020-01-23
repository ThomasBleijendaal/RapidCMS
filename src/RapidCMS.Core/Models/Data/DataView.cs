using System;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data
{
    public class DataView<TDatabaseEntity> : IDataView
    {
        public DataView(int id, string label, Expression<Func<TDatabaseEntity, bool>> expression)
        {
            Id = id;
            Label = label ?? throw new ArgumentNullException(nameof(label));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public int Id { get; private set; }
        public string Label { get; private set; }
        public Expression<Func<TDatabaseEntity, bool>> Expression { get; private set; }

        int IDataView.Id => Id;

        string IDataView.Label => Label;

        LambdaExpression IDataView.QueryExpression => Expression;
    }
}
