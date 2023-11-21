using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.Data;

public class DataView<TDatabaseEntity> : IDataView, IHasOrderByEntity<TDatabaseEntity, DataView<TDatabaseEntity>>
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

    public DataView<TDatabaseEntity> SetOrderByExpression<TValue>(Expression<Func<TDatabaseEntity, TValue>> orderByExpression, OrderByType defaultOrder = OrderByType.None)
    {
        _defaultOrderBys ??= new();

        _defaultOrderBys[PropertyMetadataHelper.GetPropertyMetadata(orderByExpression) ?? throw new InvalidOperationException("Cannot determine orderByExpression")] = defaultOrder;

        return this;
    }

    public void SetOrderBys(Dictionary<IPropertyMetadata, OrderByType> orderBys) => _defaultOrderBys = orderBys;
}
