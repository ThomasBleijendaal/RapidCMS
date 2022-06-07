using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.Data
{
    public class DataView<TDatabaseEntity> : IDataView
    {
        private Dictionary<IPropertyMetadata, OrderByType>? _defaultOrderBys;

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

        IEnumerable<KeyValuePair<IPropertyMetadata, OrderByType>> IDataView.DefaultOrderBys
            => _defaultOrderBys ?? Enumerable.Empty<KeyValuePair<IPropertyMetadata, OrderByType>>();

        /// <summary>
        /// Sets an expression that is used for ordering data in a List.
        /// 
        /// Can only be used in List views.
        /// Can only be used in Collections with a MappedBaseRepository. TDatabaseEntity must be the same as the TDatabaseEntity used by MappedBaseRepository.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="orderByExpression">Expression that is send to the IQueryable in the Repository.</param>
        /// <param name="defaultOrder">Default order (used when user opens page)</param>
        /// <returns></returns>
        public DataView<TDatabaseEntity> SetOrderByExpression<TValue>(Expression<Func<TDatabaseEntity, TValue>> orderByExpression, OrderByType defaultOrder = OrderByType.None)
        {
            _defaultOrderBys ??= new();

            _defaultOrderBys[PropertyMetadataHelper.GetPropertyMetadata(orderByExpression) ?? throw new InvalidOperationException("Cannot determine orderByExpression")] = defaultOrder;

            return this;
        }
    }
}
