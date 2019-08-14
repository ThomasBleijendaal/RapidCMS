using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public interface IDisplayFieldConfig<TEntity> : IFieldConfig<TEntity>
        where TEntity : IEntity
    {
        IDisplayFieldConfig<TEntity> SetName(string name);
        IDisplayFieldConfig<TEntity> SetDescription(string description);
        IDisplayFieldConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate);
    }

    public interface IHasButtons<TReturn>
    {
        TReturn AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false);
        TReturn AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null);
    }
}
