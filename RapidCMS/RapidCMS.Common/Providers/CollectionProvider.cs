using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Data
{
    internal class CollectionProvider : ICollectionProvider
    {
        private Dictionary<string, Collection> _collectionMap { get; set; } = new Dictionary<string, Collection>();
        private List<Collection> _collections { get; set; }

        public CollectionProvider(CmsConfig cmsConfig, IServiceProvider serviceProvider)
        {
            _collections = cmsConfig.ProcessCollections();
            FindRepositoryForCollections(serviceProvider, _collections);
        }

        Collection ICollectionProvider.GetCollection(string alias)
        {
            return _collectionMap.TryGetValue(alias, out var collection)
                ? collection
                : throw new KeyNotFoundException($"Cannot find collection with alias {alias}");
        }

        IEnumerable<Collection> ICollectionProvider.GetAllCollections()
        {
            return _collections;
        }

        IRepository? ICollectionProvider.GetRepository(string collectionAlias)
        {
            return _collectionMap.TryGetValue(collectionAlias, out var collection) ? collection.Repository : default;
        }

        private void FindRepositoryForCollections(IServiceProvider serviceProvider, IEnumerable<Collection> collections)
        {
            foreach (var collection in collections.Where(col => !col.Recursive))
            {
                // register each collection in flat dictionary
                if (!_collectionMap.TryAdd(collection.Alias, collection))
                {
                    throw new InvalidOperationException($"Duplicate collection alias '{collection.Alias}' not allowed.");
                }

                if (collection.RepositoryType != null)
                {
                    collection.Repository = (IRepository)serviceProvider.GetRequiredService(collection.RepositoryType);
                }

                FindRepositoryForCollections(serviceProvider, collection.Collections);
            }
        }
    }
}
