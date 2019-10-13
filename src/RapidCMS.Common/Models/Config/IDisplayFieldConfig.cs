using System;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.Config
{
    public interface IDisplayFieldConfig<TEntity, TValue>
        where TEntity : IEntity
    {
        /// <summary>
        /// Sets the name of this field, used in table and list views.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IDisplayFieldConfig<TEntity, TValue> SetName(string name);

        /// <summary>
        /// Sets the description of this field, displayed under the name.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        IDisplayFieldConfig<TEntity, TValue> SetDescription(string description);

        /// <summary>
        /// Sets an expression which determines whether this field should be visible.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IDisplayFieldConfig<TEntity, TValue> VisibleWhen(Func<TEntity, bool> predicate);
    }
}
