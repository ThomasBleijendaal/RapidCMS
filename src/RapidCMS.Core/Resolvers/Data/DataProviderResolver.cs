using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Validators;

namespace RapidCMS.Core.Resolvers.Data
{
    internal class DataProviderResolver : IDataProviderResolver
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionSetupResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;

        public DataProviderResolver(
            ISetupResolver<ICollectionSetup> collectionSetupResolver,
            IRepositoryResolver repositoryResolver,
            IMemoryCache memoryCache,
            IServiceProvider serviceProvider)
        {
            _collectionSetupResolver = collectionSetupResolver;
            _repositoryResolver = repositoryResolver;
            _memoryCache = memoryCache;
            _serviceProvider = serviceProvider;
        }

        public DataProvider? GetDataProvider(FieldSetup field)
        {
            if (!(field is PropertyFieldSetup propertyField && propertyField.Relation != null))
            {
                return null;
            }

            switch (propertyField.Relation)
            {
                case RepositoryRelationSetup collectionRelation:

                    var repo = collectionRelation.RepositoryAlias != null
                            ? _repositoryResolver.GetRepository(collectionRelation.RepositoryAlias)
                            : collectionRelation.CollectionAlias != null
                                ? _repositoryResolver.GetRepository(_collectionSetupResolver.ResolveSetup(collectionRelation.CollectionAlias))
                                : default;

                    if (repo == null)
                    {
                        throw new InvalidOperationException($"Field {propertyField.Property!.PropertyName} has incorrectly configure relation, cannot find repository for alias {(collectionRelation.CollectionAlias ?? collectionRelation.RepositoryAlias)}.");
                    }

                    var provider = new CollectionDataProvider(
                        repo,
                        collectionRelation.RelatedEntityType,

                        propertyField.Property!,
                        collectionRelation.RelatedElementsGetter,

                        collectionRelation.RepositoryParentSelector,
                        collectionRelation.EntityAsParent,
                        collectionRelation.IdProperty,
                        collectionRelation.DisplayProperties,

                        _memoryCache);

                    var validator = new CollectionDataValidator(propertyField.Property!);

                    return new DataProvider(
                        propertyField.Property!,
                        provider,
                        validator);

                case DataProviderRelationSetup dataProviderRelation:

                    return new DataProvider(propertyField.Property!, _serviceProvider.GetService<IDataCollection>(dataProviderRelation.DataCollectionType), default);

                default:
                    throw new InvalidOperationException();
            };
        }
    }
}
