using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Validators;

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
        /// <typeparam name="TMappedEntity"></typeparam>
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
        /// Adds an entity validator for the given entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TEntityValidator"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        IApiConfig RegisterEntityValidator<TEntity, TEntityValidator>(object? config = default)
            where TEntity : IEntity
            where TEntityValidator: IEntityValidator;

        /// <summary>
        /// Returns the registered repositories
        /// </summary>
        IEnumerable<IApiRepositoryConfig> Repositories { get; }

        /// <summary>
        /// Returns the registered data views
        /// </summary>
        IEnumerable<IApiDataViewBuilderConfig> DataViews { get; }

        /// <summary>
        /// Returns the registered file upload handlers
        /// </summary>
        IEnumerable<IFileUploadHandlerConfig> FileUploadHandlers { get; }

        /// <summary>
        /// These settings are for advanced or debugging scenarios.
        /// </summary>
        IAdvancedApiConfig Advanced { get; }
    }
}
