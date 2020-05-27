namespace RapidCMS.Core.Abstractions.Config
{
    public interface IApiConfig
    {
        /// <summary>
        /// Use this to allow anonymous users to fully use your Api. This adds a very permissive AuthorizationHandler that allows everything by anyone. 
        /// 
        /// Do not use in production.
        /// </summary>
        /// <returns></returns>
        IApiConfig AllowAnonymousUser();

        /// <summary>
        /// Registers a repository to be bound to a specific collection alias
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="collectionAlias"></param>
        /// <returns></returns>
        IApiConfig RegisterRepository<TEntity, TRepository>(string collectionAlias);
    }
}
