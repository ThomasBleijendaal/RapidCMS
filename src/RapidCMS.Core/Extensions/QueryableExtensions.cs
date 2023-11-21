using System;
using System.Linq;
using System.Linq.Expressions;

namespace RapidCMS.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TSource> WhereIfNotNull<TSource> (this IQueryable<TSource> queryable, Expression<Func<TSource, bool>>? nullablePredicate)
    {
        if (nullablePredicate != null)
        {
            return queryable.Where(nullablePredicate);
        }
        else
        {
            return queryable;
        }
    }

    public static IQueryable<TSource> WhereIfNotNull<TSource, TNullable>(this IQueryable<TSource> queryable, TNullable? nullable, Expression<Func<TSource, bool>> predicate)
        where TNullable : class
    {
        if (nullable != null)
        {
            return queryable.Where(predicate);
        }
        else
        { 
            return queryable;
        }
    }

    public static IQueryable<TSource> WhereIfNotNull<TSource, TValue>(this IQueryable<TSource> queryable, TValue? nullable, Expression<Func<TSource, bool>> predicate)
        where TValue : struct
    {
        if (nullable != null)
        {
            return queryable.Where(predicate);
        }
        else
        {
            return queryable;
        }
    }
}
