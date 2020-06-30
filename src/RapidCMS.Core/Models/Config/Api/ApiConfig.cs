using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Config.Api
{
    internal class ApiConfig : IApiConfig
    {
        internal bool AllowAnonymousUsage { get; set; } = false;
        internal Dictionary<string, ApiCollectionConfig> Collections { get; set; } = new Dictionary<string, ApiCollectionConfig>();
        internal List<Type> FileUploadHandlers { get; set; } = new List<Type>();

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

        public IApiCollectionConfig RegisterRepository<TEntity, TRepository>(string collectionAlias)
        {
            if (collectionAlias != collectionAlias.ToUrlFriendlyString())
            {
                throw new ArgumentException($"Use lowercase, hyphened strings as alias for collections, '{collectionAlias.ToUrlFriendlyString()}' instead of '{collectionAlias}'.");
            }
            if (Collections.ContainsKey(collectionAlias))
            {
                throw new NotUniqueException(nameof(collectionAlias));
            }

            return Collections[collectionAlias] = new ApiCollectionConfig
            {
                EntityType = typeof(TEntity),
                RepositoryType = typeof(TRepository)
            };
        }

        public IApiCollectionConfig RegisterRepository<TEntity, TMappedEntity, TRepository>(string collectionAlias)
        {
            if (collectionAlias != collectionAlias.ToUrlFriendlyString())
            {
                throw new ArgumentException($"Use lowercase, hyphened strings as alias for collections, '{collectionAlias.ToUrlFriendlyString()}' instead of '{collectionAlias}'.");
            }
            if (Collections.ContainsKey(collectionAlias))
            {
                throw new NotUniqueException(nameof(collectionAlias));
            }

            return Collections[collectionAlias] = new ApiCollectionConfig
            {
                EntityType = typeof(TEntity),
                DatabaseType = typeof(TMappedEntity),
                RepositoryType = typeof(TRepository)
            };
        }
    }
}
