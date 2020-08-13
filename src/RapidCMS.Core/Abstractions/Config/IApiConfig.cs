using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IApiConfig
    {
        /// <summary>
        /// Use this to allow anonymous users to fully use your Api. This adds a very permissive AuthorizationHandler that allows everything by anyone. 
        /// </summary>
        /// <returns></returns>
        IApiConfig AllowAnonymousUser();

        /// <summary>
        /// Registers a repository and creates a Controller for it
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        IApiRepositoryConfig RegisterRepository<TEntity, TRepository>()
            where TEntity : class, IEntity
            where TRepository : IRepository;

        /// <summary>
        /// Registers a mapped repository and creates a Controller for it
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        IApiRepositoryConfig RegisterRepository<TEntity, TMappedEntity, TRepository>()
            where TEntity : class, IEntity
            where TMappedEntity : class
            where TRepository : IRepository;

        /// <summary>
        /// Registers a file handler and creates a Controller for it
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        IApiConfig RegisterFileUploadHandler<THandler>()
            where THandler : IFileUploadHandler;

        /// <summary>
        /// Adds a data view builder under the collectionAlias. Data view builders allow for creating dynamic data views.
        /// </summary>
        /// <typeparam name="TDataViewBuilder"></typeparam>
        /// <returns></returns>
        IApiConfig RegisterDataViewBuilder<TDataViewBuilder>(string collectionAlias)
            where TDataViewBuilder : IDataViewBuilder;

        /// <summary>
        /// Returns the registered repositories
        /// </summary>
        IEnumerable<IApiRepositoryConfig> Repositories { get; }

        /// <summary>
        /// Returns the registered data views
        /// </summary>
        IEnumerable<IApiDataViewBuilderConfig> DataViews { get; }
    }
}
