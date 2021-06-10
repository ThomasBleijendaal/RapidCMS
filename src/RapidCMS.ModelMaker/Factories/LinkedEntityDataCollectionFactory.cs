using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Metadata;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Factories
{
    internal class LinkedEntityDataCollectionFactory : IDataCollectionFactory
    {
        private readonly CollectionsDataCollection _collectionsDataCollection;
        private readonly ISetupResolver<ICollectionSetup> _collectionSetupResolver;
        private readonly ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>> _getModelEntityByAliasCommandHandler;

        public LinkedEntityDataCollectionFactory(
            CollectionsDataCollection collectionsDataCollection,
            ISetupResolver<ICollectionSetup> collectionSetupResolver,
            ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>> getModelEntityByAliasCommandHandler)
        {
            _collectionsDataCollection = collectionsDataCollection;
            _collectionSetupResolver = collectionSetupResolver;
            _getModelEntityByAliasCommandHandler = getModelEntityByAliasCommandHandler;
        }

        public Task<RelationSetup?> GetModelEditorRelationSetupAsync() 
            => Task.FromResult<RelationSetup?>(new ConcreteDataProviderRelationSetup(_collectionsDataCollection));

        public async Task<RelationSetup?> GetModelRelationSetupAsync(IValidatorConfig? config)
        {
            if (config is not LinkedEntitiesValidationConfig linkedEntityConfig ||
                linkedEntityConfig.LinkedEntitiesCollectionAlias is not string collectionAlias)
            {
                return default;
            }

            if (collectionAlias.StartsWith(Constants.CollectionPrefix))
            {
                var response = await _getModelEntityByAliasCommandHandler.HandleAsync(new GetByAliasRequest<ModelEntity>(collectionAlias));
                if (response.Entity is ModelEntity definition && definition.PublishedProperties.FirstOrDefault(x => x.IsTitle) is PropertyModel titleProperty)
                {
                    var titlePropertyMetadata = new ExpressionMetadata<ModelMakerEntity>(titleProperty.Name, x => x.Get<string>(titleProperty.Alias));

                    return new RepositoryRelationSetup(
                        collectionAlias,
                        collectionAlias,
                        typeof(ModelMakerEntity),
                        PropertyMetadataHelper.GetPropertyMetadata(typeof(IEntity), nameof(IEntity.Id)) ?? throw new InvalidOperationException("Cannot determine idProperty for related entity"),
                        new List<IExpressionMetadata>
                        {
                            titlePropertyMetadata
                        });
                }
                else
                {
                    return default;
                }
            }
            else
            {
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
}
