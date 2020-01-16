using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IHasButtons<TReturn>
    {
        /// <summary>
        /// Adds a default button. A default button is a simple button with a fixed CrudType.
        /// </summary>
        /// <param name="type">Type of button to add</param>
        /// <param name="label">Text to display on the button</param>
        /// <param name="icon">Name of ion icon to use</param>
        /// <param name="isPrimary">Set to true to make this button to stand out</param>
        /// <returns></returns>
        TReturn AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false);

        /// <summary>
        /// Adds a custom button. A custom button is a razor component derived from BaseButton, and has a IButtonActionHandler that is invoked when the button is clicked.
        /// </summary>
        /// <typeparam name="TActionHandler">Type of the button action handler</typeparam>
        /// <param name="buttonType">Type of the razor component</param>
        /// <param name="label">Text to display on the button</param>
        /// <param name="icon">Name of ion icon to use</param>
        /// <returns></returns>
        TReturn AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
               where TActionHandler : IButtonActionHandler;

        /// <summary>
        /// Adds a pane button. A pane button is a default button that opens a modal which displays the given razor component in it. The razor component must be drived from BaseSideBar. 
        /// </summary>
        /// <param name="paneType">Type of the razor component</param>
        /// <param name="label">Text to display on the button</param>
        /// <param name="icon">Name of ion icon to use</param>
        /// <param name="defaultCrudType">Default that is provided to the BaseSideBar derived component, to use when invoking ButtonClicked.</param>
        /// <returns></returns>
        TReturn AddPaneButton(Type paneType, string? label = null, string? icon = null, CrudType? defaultCrudType = null);
    }
}
