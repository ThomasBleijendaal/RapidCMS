using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.Repositories
{
    internal class ModelMakerRepository : IRepository
    {
        private readonly ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>> _getDefinitionByIdHandler;
        private readonly ICommandHandler<RemoveRequest<ModelMakerEntity>, ConfirmResponse> _removeCommandHandler;
        private readonly ICommandHandler<GetAllRequest<ModelMakerEntity>, EntitiesResponse<ModelMakerEntity>> _getAllEntitiesCommandHandler;
        private readonly ICommandHandler<GetByIdRequest<ModelMakerEntity>, EntityResponse<ModelMakerEntity>> _getEntityCommandHandler;
        private readonly ICommandHandler<InsertRequest<ModelMakerEntity>, EntityResponse<ModelMakerEntity>> _insertEntityCommandHandler;
        private readonly ICommandHandler<PublishRequest<ModelMakerEntity>, ConfirmResponse> _publishEntityCommandHandler;
        private readonly IModelMakerConfig _config;
        private readonly IServiceProvider _serviceProvider;

        public ModelMakerRepository(
            ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>> getDefinitionByIdHandler,
            ICommandHandler<RemoveRequest<ModelMakerEntity>, ConfirmResponse> removeCommandHandler,
            ICommandHandler<GetAllRequest<ModelMakerEntity>, EntitiesResponse<ModelMakerEntity>> getAllEntitiesCommandHandler,
            ICommandHandler<GetByIdRequest<ModelMakerEntity>, EntityResponse<ModelMakerEntity>> getEntityCommandHandler,
            ICommandHandler<InsertRequest<ModelMakerEntity>, EntityResponse<ModelMakerEntity>> insertEntityCommandHandler,
            ICommandHandler<PublishRequest<ModelMakerEntity>, ConfirmResponse> publishEntityCommandHandler,
            IModelMakerConfig config,
            IServiceProvider serviceProvider)
        {
            _getDefinitionByIdHandler = getDefinitionByIdHandler;
            _removeCommandHandler = removeCommandHandler;
            _getAllEntitiesCommandHandler = getAllEntitiesCommandHandler;
            _getEntityCommandHandler = getEntityCommandHandler;
            _insertEntityCommandHandler = insertEntityCommandHandler;
            _publishEntityCommandHandler = publishEntityCommandHandler;
            _config = config;
            _serviceProvider = serviceProvider;
        }

        public Task AddAsync(IRelatedViewContext viewContext, string id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(string id, IViewContext viewContext)
        {
            await _removeCommandHandler.HandleAsync(new RemoveRequest<ModelMakerEntity>(id, viewContext.CollectionAlias));
        }

        public async Task<IEnumerable<IEntity>> GetAllAsync(IViewContext viewContext, IQuery query)
        {
            var response = await _getAllEntitiesCommandHandler.HandleAsync(new GetAllRequest<ModelMakerEntity>(viewContext.CollectionAlias));

            return response.Entities;
        }

        public Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelatedViewContext viewContext, IQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelatedViewContext viewContext, IQuery query)
        {
            throw new NotImplementedException();
        }

        public async Task<IEntity?> GetByIdAsync(string id, IViewContext viewContext)
        {
            var response = await _getEntityCommandHandler.HandleAsync(new GetByIdRequest<ModelMakerEntity>(id, viewContext.CollectionAlias));

            return response.Entity;
        }

        public async Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelMakerEntity> typedEditContext)
            {
                var entity = typedEditContext.Entity;

                entity.Alias = typedEditContext.CollectionAlias;

                await ValidateEntityAsync(typedEditContext, entity);

                var response = await _insertEntityCommandHandler.HandleAsync(new InsertRequest<ModelMakerEntity>(entity));

                return response.Entity;
            }

            return default;
        }

        public Task<IEntity> NewAsync(IViewContext viewContext, Type? variantType)
        {
            return Task.FromResult<IEntity>(new ModelMakerEntity());
        }

        public Task RemoveAsync(IRelatedViewContext viewContext, string id)
        {
            throw new NotImplementedException();
        }

        public Task ReorderAsync(string? beforeId, string id, IViewContext viewContext)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelMakerEntity> typedEditContext)
            {
                var entity = typedEditContext.Entity;

                await ValidateEntityAsync(typedEditContext, entity);

                await _publishEntityCommandHandler.HandleAsync(new PublishRequest<ModelMakerEntity>(entity));
            }
        }

        private async Task ValidateEntityAsync(IEditContext<ModelMakerEntity> editContext, ModelMakerEntity entity)
        {
            var modelDefinition = await _getDefinitionByIdHandler.HandleAsync(new GetByAliasRequest<ModelEntity>(editContext.CollectionAlias));

            if (modelDefinition.Entity != null)
            {
                foreach (var property in modelDefinition.Entity.PublishedProperties)
                {
                    foreach (var validation in property.Validations.Where(x => x.Config?.IsEnabled == true))
                    {
                        var validatorConfig = _config.Validators.First(x => x.Alias == validation.Alias);

                        var validator = _serviceProvider.GetService<IValidator>(validatorConfig.Validator);

                        if (!await validator.IsValid(entity.Get(property.Alias), validation.Config!))
                        {
                            editContext.AddValidationError(property.Alias, await validator.ErrorMessage(validation.Config!));
                        }
                    }
                }
            }

            editContext.EnforceValidEntity();
        }
    }
}
