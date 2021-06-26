using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Resolvers.Data
{
    internal class DataProviderResolver : IDataProviderResolver
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionSetupResolver;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        public DataProviderResolver(
            ISetupResolver<ICollectionSetup> collectionSetupResolver,
            IConcurrencyService concurrencyService,
            IRepositoryResolver repositoryResolver,
            IMediator mediator,
            IServiceProvider serviceProvider)
        {
            _collectionSetupResolver = collectionSetupResolver;
            _concurrencyService = concurrencyService;
            _repositoryResolver = repositoryResolver;
            _mediator = mediator;
            _serviceProvider = serviceProvider;
        }

        public async Task<FormDataProvider?> GetDataProviderAsync(IFieldSetup field)
        {
            if (!(field is PropertyFieldSetup propertyField && propertyField.Relation != null))
            {
                return null;
            }

            switch (propertyField.Relation)
            {
                case RepositoryRelationSetup collectionRelation:

                    var collectionSetup = collectionRelation.CollectionAlias == null
                        ? default
                        : await _collectionSetupResolver.ResolveSetupAsync(collectionRelation.CollectionAlias);

                    var repo = collectionRelation.RepositoryAlias != null
                            ? _repositoryResolver.GetRepository(collectionRelation.RepositoryAlias)
                            : collectionSetup != null
                                ? _repositoryResolver.GetRepository(collectionSetup)
                                : default;

                    if (repo == null)
                    {
                        throw new InvalidOperationException($"Field {propertyField.Property!.PropertyName} has incorrectly configure relation, cannot find repository for alias {(collectionRelation.CollectionAlias ?? collectionRelation.RepositoryAlias)}.");
                    }

                    // TODO: investigate whether this can be moved to Setup to allow for better caching
                    var idProperty = collectionRelation.IdProperty
                        ?? collectionSetup?.ElementSetup?.IdProperty
                        ?? throw new InvalidOperationException($"Field {propertyField.Property!.PropertyName} has incorrect Id property metadata.");

                    var displayProperties = collectionRelation.DisplayProperties
                        ?? collectionSetup?.ElementSetup?.DisplayProperties
                        ?? throw new InvalidOperationException($"Field {propertyField.Property!.PropertyName} has incorrect display properties metadata.");

                    var relatedElementGetter = collectionRelation.RelatedElementsGetter
                        ?? ((collectionRelation.IsRelationToMany && propertyField.Property != null && propertyField.Property.PropertyType.IsAssignableTo(typeof(IEnumerable<IEntity>)))
                            ? PropertyMetadataHelper.GetPropertyMetadata<IEnumerable<IEntity>, IEnumerable<object?>>(x => x.Select(idProperty.Getter))
                            : default);

                    var provider = new CollectionDataProvider(
                        repo,
                        _concurrencyService,
                        collectionRelation.RepositoryAlias,
                        collectionRelation.CollectionAlias,
                        relatedElementGetter,
                        collectionRelation.EntityAsParent,
                        collectionRelation.RepositoryParentSelector,
                        idProperty,
                        displayProperties,
                        collectionRelation.RelatedEntityType ?? collectionSetup?.EntityVariant.Type ?? typeof(object),
                        propertyField.Property!,
                        _mediator);

                    return new FormDataProvider(
                        propertyField.Property!,
                        provider);

                case DataProviderRelationSetup dataProviderRelation:

                    var dataCollection = _serviceProvider.GetService<IDataCollection>(dataProviderRelation.DataCollectionType);
                    if (dataProviderRelation.Configuration != null)
                    {
                        dataCollection.Configure(dataProviderRelation.Configuration);
                    }

                    return new FormDataProvider(propertyField.Property!, dataCollection);

                case ConcreteDataProviderRelationSetup concreteDataProvider:

                    return new FormDataProvider(propertyField.Property!, concreteDataProvider.DataCollection);

                default:
                    throw new InvalidOperationException();
            };
        }
    }
}
