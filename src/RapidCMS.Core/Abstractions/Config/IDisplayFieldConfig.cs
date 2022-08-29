using System;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IDisplayFieldConfig<TEntity, TValue>
        : IHasOrderBy<TEntity, IDisplayFieldConfig<TEntity, TValue>>,
        IHasNameDescription<IDisplayFieldConfig<TEntity, TValue>>,
        IHasConfigurability<TEntity, IDisplayFieldConfig<TEntity, TValue>>
        where TEntity : IEntity
    {
        /// <summary>
        /// Sets the type of build-in display used for this field.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IDisplayFieldConfig<TEntity, TValue> SetType(DisplayType type);

        /// <summary>
        /// Sets the type of custom razor component used for this field.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IDisplayFieldConfig<TEntity, TValue> SetType(Type type);

        /// <summary>
        /// Sets an expression which determines whether this field should be visible.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IDisplayFieldConfig<TEntity, TValue> VisibleWhen(Func<TEntity, EntityState, bool> predicate);
    }
}
