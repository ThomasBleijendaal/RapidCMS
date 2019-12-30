using System;

namespace RapidCMS.Core.Interfaces.Config
{
    public interface ICmsConfig : ICollectionConfig
    {
        /// <summary>
        /// Adds a section to the list of components to draw on the dashboard, the homepage of the CMS.
        /// Use the edit flag to indicate whether this should be the editor or the view variant of the collection.
        /// </summary>
        /// <param name="collectionAlias">Alias of a collection</param>
        /// <param name="edit">Set to true to use the ListEditor, otherwise the ListView is used</param>
        /// <returns></returns>
        ICmsConfig AddDashboardSection(string collectionAlias, bool edit = false);

        /// <summary>
        /// Adds a razor component to the list of components to draw on the dashboard, the homepage of the CMS. 
        /// </summary>
        /// <param name="customDashboardSectionType">Type of the razor component to draw.</param>
        /// <returns></returns>
        ICmsConfig AddDashboardSection(Type customDashboardSectionType);

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
    }
}
