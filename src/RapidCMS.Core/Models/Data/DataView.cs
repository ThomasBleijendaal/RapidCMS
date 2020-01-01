using System;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data
{
    internal class DataView<TEntity> : IDataView
    {
        public DataView(int id, string label, Expression<Func<TEntity, bool>> expression)
        {
            Id = id;
            Label = label ?? throw new ArgumentNullException(nameof(label));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public int Id { get; }
        public string Label { get; set; }
        public Expression<Func<TEntity, bool>> Expression { get; set; }

        int IDataView.Id => Id;

        string IDataView.Label => Label;

        LambdaExpression IDataView.QueryExpression => Expression;
    }
}
