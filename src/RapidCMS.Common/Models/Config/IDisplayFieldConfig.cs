using System;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface IDisplayFieldConfig<TEntity, TValue> : IFieldConfig<TEntity>
        where TEntity : IEntity
    {
        IDisplayFieldConfig<TEntity, TValue> SetName(string name);
        IDisplayFieldConfig<TEntity, TValue> SetDescription(string description);
        IDisplayFieldConfig<TEntity, TValue> VisibleWhen(Func<TEntity, bool> predicate);
    }
}
