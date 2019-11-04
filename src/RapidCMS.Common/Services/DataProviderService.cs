using System;
using Microsoft.Extensions.Caching.Memory;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Providers;

namespace RapidCMS.Common.Services
{
    internal class DataProviderService : IDataProviderService
    {
        private readonly ICollectionProvider _collectionProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;

        public DataProviderService(ICollectionProvider collectionProvider, IMemoryCache memoryCache, IServiceProvider serviceProvider)
        {
            _collectionProvider = collectionProvider;
            _memoryCache = memoryCache;
            _serviceProvider = serviceProvider;
        }

        public DataProvider? GetDataProvider(Field field)
        {
            if (!(field is PropertyField propertyField && propertyField.Relation != null))
            {
                return null;
            }

            switch (propertyField.Relation)
            {
                case CollectionRelation collectionRelation:

                    var repo = _collectionProvider.GetRepository(collectionRelation.CollectionAlias);
                    if (repo == null)
                    {
                        throw new InvalidOperationException($"Field {propertyField.Property.PropertyName} has incorrectly configure relation, cannot find repository for collection alias {collectionRelation.CollectionAlias}.");
                    }

                    var provider = new CollectionDataProvider(
                        repo,
                        collectionRelation.RelatedEntityType,

                        propertyField.Property,
                        collectionRelation.RelatedElementsGetter,

                        collectionRelation.RepositoryParentSelector,
                        collectionRelation.IdProperty,
                        collectionRelation.DisplayProperties,

                        _memoryCache);

                    var validator = new CollectionDataValidator(propertyField.Property);

                    return new DataProvider(
                        propertyField.Property,
                        provider,
                        validator);

                case DataProviderRelation dataProviderRelation:

                    return new DataProvider(propertyField.Property, _serviceProvider.GetService<IDataCollection>(dataProviderRelation.DataCollectionType), default);

                default:
                    throw new InvalidOperationException();
            };
        }
    }
}
