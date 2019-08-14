using System;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface IDisplayFieldConfig<TEntity> : IFieldConfig<TEntity>
        where TEntity : IEntity
    {
        IDisplayFieldConfig<TEntity> SetName(string name);
        IDisplayFieldConfig<TEntity> SetDescription(string description);
        IDisplayFieldConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate);
    }
}
