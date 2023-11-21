using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config;

public interface IHasDisplayTable<TEntity, TReturn>
    where TEntity : IEntity
{
    /// <summary>
    /// Adds a row to the current table.
    /// </summary>
    /// <param name="configure">Action to configure the section.</param>
    /// <returns></returns>
    TReturn AddRow(Action<IDisplayPaneConfig<TEntity>> configure);

    /// <summary>
    /// Adds a row to the current table.
    /// </summary>
    /// <param name="customSectionType">Custom razor component that will be used to render this section. Must be derived from BaseSection.</param>
    /// <param name="configure">Action to configure the section.</param>
    /// <returns></returns>
    TReturn AddRow(Type customSectionType, Action<IDisplayPaneConfig<TEntity>> configure);

    /// <summary>
    /// Adds a row to the current table, tailored for the specific TDerivedEntity EntityVariant. Only entities of that type will use this row.
    /// </summary>
    /// <typeparam name="TDerivedEntity">Type of the EntityVariant.</typeparam>
    /// <param name="configure">Action to configure the section.</param>
    /// <returns></returns>
    TReturn AddRow<TDerivedEntity>(Action<IDisplayPaneConfig<TDerivedEntity>> configure)
        where TDerivedEntity : TEntity;

    /// <summary>
    /// Adds a row to the current table, tailored for the specific TDerivedEntity. Only entities of that type will use this section.
    /// </summary>
    /// <typeparam name="TDerivedEntity">Type of the EntityVariant.</typeparam>
    /// <param name="customSectionType">Custom razor component that will be used to render this section. Must be derived from BaseSection.</param>
    /// <param name="configure">Action to configure the section.</param>
    /// <returns></returns>
    TReturn AddRow<TDerivedEntity>(Type? customSectionType, Action<IDisplayPaneConfig<TDerivedEntity>> configure)
        where TDerivedEntity : TEntity;
}
