using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
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
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        public DataProviderResolver(
            ISetupResolver<ICollectionSetup> collectionSetupResolver,
            IRepositoryResolver repositoryResolver,
            IMediator mediator,
            IServiceProvider serviceProvider)
        {
            _collectionSetupResolver = collectionSetupResolver;
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

                    var repo = collectionRelation.RepositoryAlias != null
                            ? _repositoryResolver.GetRepository(collectionRelation.RepositoryAlias)
                            : collectionRelation.CollectionAlias != null
                                ? _repositoryResolver.GetRepository(
                                    await _collectionSetupResolver.ResolveSetupAsync(collectionRelation.CollectionAlias))
                                : default;

                    if (repo == null)
                    {
                        throw new InvalidOperationException($"Field {propertyField.Property!.PropertyName} has incorrectly configure relation, cannot find repository for alias {(collectionRelation.CollectionAlias ?? collectionRelation.RepositoryAlias)}.");
                    }

                    var provider = new CollectionDataProvider(
                        repo,
                        collectionRelation,
                        propertyField.Property!,
                        _mediator);

                    var validator = new RelationValidationAttributeValidator(propertyField.Property!);

                    return new FormDataProvider(
                        propertyField.Property!,
                        provider,
                        validator);

                case DataProviderRelationSetup dataProviderRelation:

                    return new FormDataProvider(propertyField.Property!, _serviceProvider.GetService<IDataCollection>(dataProviderRelation.DataCollectionType), default);

                case ConcreteDataProviderRelationSetup concreteDataProvider:

                    return new FormDataProvider(propertyField.Property!, concreteDataProvider.DataCollection, default);

                default:
                    throw new InvalidOperationException();
            };
        }
    }
}
