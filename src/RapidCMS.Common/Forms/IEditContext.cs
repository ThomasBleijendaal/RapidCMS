using System;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Forms
{
    public interface IEditContext
    {
        EntityState EntityState { get; }

        IRelationContainer GetRelationContainer();
        bool? IsModified<TEntity, TValue>(Expression<Func<TEntity, TValue>> property);
        bool? IsModified(string propertyName);
    }
}
