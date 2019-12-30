using System;
using RapidCMS.Core.Interfaces.Data;

namespace RapidCMS.Core.Interfaces.Config
{
    public interface IHasDisplayPanes<TEntity, TReturn>
        where TEntity : IEntity
    {
        /// <summary>
        /// Adds a section to the current view.
        /// </summary>
        /// <param name="configure">Action to configure the section.</param>
        /// <returns></returns>
        TReturn AddSection(Action<IDisplayPaneConfig<TEntity>> configure);

        /// <summary>
        /// Adds a section to the current view.
        /// </summary>
        /// <param name="customSectionType">Custom razor component that will be used to render this section. Must be derived from BaseSection.</param>
        /// <param name="configure">Action to configure the section.</param>
        /// <returns></returns>
        TReturn AddSection(Type customSectionType, Action<IDisplayPaneConfig<TEntity>>? configure = null);

        /// <summary>
        /// Adds a section to the current view, tailored for the specific TDerivedEntity EntityVariant. Only entities of that type will use this section.
        /// </summary>
        /// <typeparam name="TDerivedEntity">Type of the EntityVariant.</typeparam>
        /// <param name="configure">Action to configure the section.</param>
        /// <returns></returns>
        TReturn AddSection<TDerivedEntity>(Action<IDisplayPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity;

        /// <summary>
        /// Adds a section to the current view, tailored for the specific TDerivedEntity. Only entities of that type will use this section.
        /// </summary>
        /// <typeparam name="TDerivedEntity">Type of the EntityVariant.</typeparam>
        /// <param name="customSectionType">Custom razor component that will be used to render this section. Must be derived from BaseSection.</param>
        /// <param name="configure">Action to configure the section.</param>
        /// <returns></returns>
        TReturn AddSection<TDerivedEntity>(Type? customSectionType, Action<IDisplayPaneConfig<TDerivedEntity>>? configure)
            where TDerivedEntity : TEntity;
    }
}
