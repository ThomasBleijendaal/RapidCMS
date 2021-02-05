using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;
using RapidCMS.Repositories.ApiBridge;

namespace RapidCMS.Core.Models.Config.Api
{
    internal class ApiConfig : IApiConfig
    {
        internal bool AllowAnonymousUsage { get; set; } = false;
        internal List<ApiRepositoryConfig> Repositories { get; set; } = new List<ApiRepositoryConfig>();
        internal List<IApiDataViewBuilderConfig> DataViews { get; set; } = new List<IApiDataViewBuilderConfig>();
        internal List<Type> FileUploadHandlers { get; set; } = new List<Type>();

        IEnumerable<IApiRepositoryConfig> IApiConfig.Repositories => Repositories;
        IEnumerable<IApiDataViewBuilderConfig> IApiConfig.DataViews => DataViews;

        public IApiConfig AllowAnonymousUser()
        {
            AllowAnonymousUsage = true;
            return this;
        }

        public IApiConfig RegisterFileUploadHandler<THandler>() where THandler : IFileUploadHandler
        {
            FileUploadHandlers.Add(typeof(THandler));
            return this;
        }

        public IApiRepositoryConfig RegisterRepository<TEntity, TRepository>()
            where TEntity : class, IEntity
            where TRepository : IRepository
        {
            var fullType = typeof(ApiRepository<TEntity, TRepository>);
            var alias = AliasHelper.GetRepositoryAlias(fullType);

            if (Repositories.Any(x => x.Alias == alias))
            {
                throw new NotUniqueException(nameof(TRepository));
            }

            var config = new ApiRepositoryConfig
            {
                Alias = alias,
                EntityType = typeof(TEntity),
                RepositoryType = typeof(TRepository),
                ApiRepositoryType = fullType
            };
            Repositories.Add(config);
            return config;
        }

        public IApiRepositoryConfig RegisterRepository<TEntity, TMappedEntity, TRepository>()
            where TEntity : class, IEntity
            where TMappedEntity : class
            where TRepository : IRepository
        {
            var fullType = typeof(ApiMappedRepository<TEntity, TMappedEntity, TRepository>);
            var alias = AliasHelper.GetRepositoryAlias(fullType);

            if (Repositories.Any(x => x.Alias == alias))
            {
                throw new NotUniqueException(nameof(TRepository));
            }

            var config = new ApiRepositoryConfig
            {
                Alias = alias,
                EntityType = typeof(TEntity),
                DatabaseType = typeof(TMappedEntity),
                RepositoryType = typeof(TRepository),
                ApiRepositoryType = fullType
            };
            Repositories.Add(config);
            return config;
        }

        public IApiConfig RegisterDataViewBuilder<TDataViewBuilder>(string collectionAlias)
             where TDataViewBuilder : IDataViewBuilder
        {
            if (DataViews.Any(x => x.Alias == collectionAlias))
            {
                throw new NotUniqueException(nameof(collectionAlias));
            }

            DataViews.Add(new ApiDataViewBuilderConfig
            {
                Alias = collectionAlias,
                DataViewBuilder = typeof(TDataViewBuilder)
            });

            return this;
        }
    }
}
