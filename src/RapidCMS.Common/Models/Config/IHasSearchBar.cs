namespace RapidCMS.Common.Models.Config
{
    public interface IHasSearchBar<TReturn>
    {
        /// <summary>
        /// Sets the visibility of the search bar atop of the ListEditor.
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        TReturn SetSearchBarVisibility(bool visible);
    }
}
