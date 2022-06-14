using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.Config
{
    internal class OrderByConfig<TEntity> : IOrderByConfig<TEntity>
    {
        public Dictionary<IPropertyMetadata, OrderByType> DefaultOrderBys { get; set; } = new();

        public IOrderByConfig<TEntity> SetOrderByExpression<TValue>(Expression<Func<TEntity, TValue>> orderByExpression, OrderByType defaultOrder = OrderByType.None)
        {
            DefaultOrderBys[PropertyMetadataHelper.GetPropertyMetadata(orderByExpression) ?? throw new InvalidOperationException("Cannot determine orderByExpression")] = defaultOrder;

            return this;
        }

        public IOrderByConfig<TEntity> SetOrderByExpression<TDatabaseEntity, TValue>(Expression<Func<TDatabaseEntity, TValue>> orderByExpression, OrderByType defaultOrder = OrderByType.None)
        {
            DefaultOrderBys[PropertyMetadataHelper.GetPropertyMetadata(orderByExpression) ?? throw new InvalidOperationException("Cannot determine orderByExpression")] = defaultOrder;

            return this;
        }
    }
}
