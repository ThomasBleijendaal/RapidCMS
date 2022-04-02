using System;
using System.Collections.Generic;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IPageConfig : ITreeElementConfig
    {
        /// <summary>
        /// Adds a section to the list of components to draw on this page.
        /// Use the edit flag to indicate whether this should be the editor or the view variant of the collection.
        /// </summary>
        /// <param name="collectionAlias">Alias of a collection</param>
        /// <param name="edit">Set to true to use the ListEditor, otherwise the ListView is used</param>
        /// <returns></returns>
        IPageConfig AddSection(string collectionAlias, bool edit = false);

        /// <summary>
        /// Adds a razor component to the list of components to draw on the dashboard, the homepage of the CMS. 
        /// </summary>
        /// <param name="customSectionType">Type of the razor component to draw.</param>
        /// <returns></returns>
        IPageConfig AddSection(Type customSectionType);

        string Icon { get; }
        string Color { get; }
    }
}
