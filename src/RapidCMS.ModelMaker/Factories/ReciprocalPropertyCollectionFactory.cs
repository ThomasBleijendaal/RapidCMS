using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.Factories
{
    internal class ReciprocalPropertyCollectionFactory : IDataCollectionFactory
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionSetupResolver;
        private readonly ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> _modelResolver;

        public ReciprocalPropertyCollectionFactory(
            ISetupResolver<ICollectionSetup> collectionSetupResolver,
            ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> modelResolver)
        {
            _collectionSetupResolver = collectionSetupResolver;
            _modelResolver = modelResolver;
        }

        public Task<RelationSetup?> GetModelEditorRelationSetupAsync()
            => Task.FromResult<RelationSetup?>(
                new ConcreteDataProviderRelationSetup(
                    new ReciprocalPropertyDataCollection(
                        _collectionSetupResolver,
                        _modelResolver)));

        public Task<RelationSetup?> GetModelRelationSetupAsync(IValidatorConfig config)
        {
            return Task.FromResult<RelationSetup?>(default);
        }
    }
}
