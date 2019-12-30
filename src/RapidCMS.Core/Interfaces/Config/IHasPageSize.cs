namespace RapidCMS.Core.Interfaces.Config
{
    public interface IHasPageSize<TReturn>
    {
        /// <summary>
        /// Sets the pagesize of the ListEditor. 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        TReturn SetPageSize(int pageSize);
    }
}
