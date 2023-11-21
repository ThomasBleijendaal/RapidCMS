﻿using System;
using System.Linq.Expressions;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Config;

public interface IHasOrderByDatabaseEntity<TReturn>
{
    /// <summary>
    /// Sets an expression that is used for ordering data in a List.
    /// 
    /// Can only be used in List views.
    /// Can only be used in Collections with a MappedBaseRepository. TDatabaseEntity must be the same as the TDatabaseEntity used by MappedBaseRepository.
    /// <br /><br />
    /// When using order by expressions in DataViews / DataViewBuilders, the List view must have a corresponding OrderByExpression set on the correct field. 
    /// </summary>
    /// <typeparam name="TDatabaseEntity">Entity used by the Repository to access the data store</typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="orderByExpression">Expression that is send to the IQueryable in the Repository.</param>
    /// <param name="defaultOrder">Default order (used when user opens page)</param>
    /// <returns></returns>
    TReturn SetOrderByExpression<TDatabaseEntity, TValue>(Expression<Func<TDatabaseEntity, TValue>> orderByExpression, OrderByType defaultOrder = OrderByType.None);
}
