namespace RapidCMS.Core.Abstractions.Config
{
    public interface IHasOrderBy<TEntity, TReturn> : 
        IHasOrderByEntity<TEntity, TReturn>, 
        IHasOrderByDatabaseEntity<TReturn>
    {
        
    }
}
