using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IHasEditorPanes<TEntity, TReturn>
        where TEntity : IEntity
    {
        /// <summary>
        /// Adds a section to the current editor.
        /// 
        /// In a ListEditor, a section is a seperate block.
        /// In a TableEditor, a section is a row.
        /// </summary>
        /// <param name="configure">Action to configure the section.</param>
        /// <returns></returns>
        TReturn AddSection(Action<IEditorPaneConfig<TEntity>> configure);

        /// <summary>
        /// Adds a section to the current editor.
        /// 
        /// In a ListEditor, a section is a seperate block.
        /// In a TableEditor, a section is a row.
        /// </summary>
        /// <param name="customSectionType">Custom razor component that will be used to render this section. Must be derived from BaseSection.</param>
        /// <param name="configure">Action to configure the section.</param>
        /// <returns></returns>
        TReturn AddSection(Type customSectionType, Action<IEditorPaneConfig<TEntity>>? configure = null);

        /// <summary>
        /// Adds a section to the current editor, tailored for the specific TDerivedEntity EntityVariant. Only entities of that type will use this section.
        /// 
        /// In a ListEditor, a section is a seperate block.
        /// In a TableEditor, a section is a row.
        /// </summary>
        /// <typeparam name="TDerivedEntity">Type of the EntityVariant.</typeparam>
        /// <param name="configure">Action to configure the section.</param>
        /// <returns></returns>
        TReturn AddSection<TDerivedEntity>(Action<IEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity;

        /// <summary>
        /// Adds a section to the current editor, tailored for the specific TDerivedEntity. Only entities of that type will use this section.
        /// 
        /// In a ListEditor, a section is a seperate block.
        /// In a TableEditor, a section is a row.
        /// </summary>
        /// <typeparam name="TDerivedEntity">Type of the EntityVariant.</typeparam>
        /// <param name="customSectionType">Custom razor component that will be used to render this section. Must be derived from BaseSection.</param>
        /// <param name="configure">Action to configure the section.</param>
        /// <returns></returns>
        TReturn AddSection<TDerivedEntity>(Type? customSectionType, Action<IEditorPaneConfig<TDerivedEntity>>? configure)
            where TDerivedEntity : TEntity;
    }
}
