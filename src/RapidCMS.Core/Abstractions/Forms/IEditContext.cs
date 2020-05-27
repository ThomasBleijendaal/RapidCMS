using System;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Forms
{
    public interface IEditContext
    {

    }

    public interface IEditContext<TEntity> : IEditContext
        where TEntity : IEntity
    {
        UsageType UsageType { get; }
        EntityState EntityState { get; }

        TEntity Entity { get; }
        IParent? Parent { get; }

        IRelationContainer GetRelationContainer();
        bool? IsModified<TValue>(Expression<Func<TEntity, TValue>> property);
        bool? IsModified(string propertyName);

        bool? IsValid<TValue>(Expression<Func<TEntity, TValue>> property);
        bool? IsValid(string propertyName);

        bool? WasValidated<TValue>(Expression<Func<TEntity, TValue>> property);
        bool? WasValidated(string propertyName);
    }
}
