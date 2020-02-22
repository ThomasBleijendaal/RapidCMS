using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
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

        string Name { get; }
        string Icon { get; }

        /// <summary>
        /// Registered sections of the page.
        /// </summary>
        IEnumerable<CustomTypeRegistrationConfig> SectionRegistrations { get; }
    }

    public interface ICmsConfig : ICollectionConfig
    {
        /// <summary>
        /// Adds a collection to the CMS.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity of this collection</typeparam>
        /// <typeparam name="TRepository">Type of the repository this collection will use</typeparam>
        /// <param name="alias">Alias of the collection</param>
        /// <param name="name">Human readable name of this collection</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICollectionConfig<TEntity> AddCollection<TEntity, TRepository>(string alias, string name, Action<ICollectionConfig<TEntity>> configure)
            where TEntity : class, IEntity
            where TRepository : IRepository;

        /// <summary>
        /// Adds a collection to the CMS.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity of this collection</typeparam>
        /// <typeparam name="TRepository">Type of the repository this collection will use</typeparam>
        /// <param name="alias">Alias of the collection</param>
        /// <param name="icon">Icon for this collection</param>
        /// <param name="name">Human readable name of this collection</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICollectionConfig<TEntity> AddCollection<TEntity, TRepository>(string alias, string? icon, string name, Action<ICollectionConfig<TEntity>> configure)
            where TEntity : class, IEntity
            where TRepository : IRepository;

        /// <summary>
        /// Adds a page to the CMS.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICmsConfig AddPage(string name, Action<IPageConfig> configure);

        /// <summary>
        /// Add a page to the CMS.
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="name"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ICmsConfig AddPage(string icon, string name, Action<IPageConfig> configure);

        /// <summary>
        /// The CMS homepage.
        /// </summary>
        IPageConfig Dashboard { get; }

        /// <summary>
        /// Use this to allow anonymous users to fully use your CMS. This adds a very permissive AuthorizationHandler that allows everything by anyone. 
        /// 
        /// Do not use in production.
        /// </summary>
        /// <returns></returns>
        ICmsConfig AllowAnonymousUser();

        /// <summary>
        /// Draws the given razor component as login screen.
        /// </summary>
        /// <param name="loginType">Type of razor component.</param>
        /// <returns></returns>
        ICmsConfig SetCustomLoginScreen(Type loginType);

        /// <summary>
        /// Draws the given razor component in the top bar of the CMS. Use this to display the status of the currently signed in user, and the possibility to sign out.
        /// </summary>
        /// <param name="loginType">Type of razor component.</param>
        /// <returns></returns>
        ICmsConfig SetCustomLoginStatus(Type loginType);

        /// <summary>
        /// Sets the name of title in the top left bar of the CMS. Defaults to RapidCMS.
        /// </summary>
        /// <param name="siteName">Name of your CMS</param>
        /// <returns></returns>
        ICmsConfig SetSiteName(string siteName);

        /// <summary>
        /// These settings are for advanced or debugging scenarios.
        /// </summary>
        IAdvancedCmsConfig Advanced { get; }
    }
}
