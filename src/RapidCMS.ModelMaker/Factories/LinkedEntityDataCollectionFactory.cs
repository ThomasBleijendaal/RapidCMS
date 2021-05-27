using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.Factories;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Factories
{
    internal class LinkedEntityDataCollectionFactory : IDataCollectionFactory
    {
        private readonly CollectionsDataCollection _collectionsDataCollection;
        private readonly ISetupResolver<ICollectionSetup> _collectionSetupResolver;

        public LinkedEntityDataCollectionFactory(
            CollectionsDataCollection collectionsDataCollection,
            ISetupResolver<ICollectionSetup> collectionSetupResolver)
        {
            _collectionsDataCollection = collectionsDataCollection;
            _collectionSetupResolver = collectionSetupResolver;
        }

        public Task<RelationSetup?> GetModelEditorRelationSetupAsync() 
            => Task.FromResult<RelationSetup?>(new ConcreteDataProviderRelationSetup(_collectionsDataCollection));

        public async Task<RelationSetup?> GetModelRelationSetupAsync(IValidatorConfig? config)
        {
            if (config is not LinkedEntityValidationConfig linkedEntityConfig ||
                linkedEntityConfig.CollectionAlias is not string collectionAlias)
            {
                return default;
            }

            // TODO: this method does not handle recursion well (will trigger a endless loop)
            var collectionSetup = await _collectionSetupResolver.ResolveSetupAsync(collectionAlias);

            return new RepositoryRelationSetup(
                collectionSetup.RepositoryAlias,
                collectionAlias,
                collectionSetup.EntityVariant.Type,
                PropertyMetadataHelper.GetPropertyMetadata(typeof(IEntity), nameof(IEntity.Id)) ?? throw new InvalidOperationException("Cannot determine idProperty for related entity"),
                new List<IExpressionMetadata>
                {
                    collectionSetup.TreeView?.Name ?? throw new InvalidOperationException("Related entity must have tree view to be referenced in model maker entity")
                });
        }
    }
}
