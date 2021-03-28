using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.Repositories
{
    internal class ModelRepository : IRepository
    {
        private readonly ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse> _removeCommandHandler;
        private readonly ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>> _getAllEntitiesCommandHandler;
        private readonly ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> _getEntityCommandHandler;
        private readonly ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>> _insertEntityCommandHandler;
        private readonly ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse> _updateEntityCommandHandler;

        public ModelRepository(
            ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse> removeCommandHandler,
            ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>> getAllEntitiesCommandHandler,
            ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> getEntityCommandHandler,
            ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>> insertEntityCommandHandler,
            ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse> updateEntityCommandHandler)
        {
            _removeCommandHandler = removeCommandHandler;
            _getAllEntitiesCommandHandler = getAllEntitiesCommandHandler;
            _getEntityCommandHandler = getEntityCommandHandler;
            _insertEntityCommandHandler = insertEntityCommandHandler;
            _updateEntityCommandHandler = updateEntityCommandHandler;
        }

        public Task AddAsync(IRelated related, string id)
        {
            throw new NotSupportedException();
        }

        public async Task DeleteAsync(string id, IParent? parent)
        {
            await _removeCommandHandler.HandleAsync(new RemoveRequest<ModelEntity>(id));
        }

        public async Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query)
        {
            var response = await _getAllEntitiesCommandHandler.HandleAsync(new GetAllRequest<ModelEntity>());

            return response.Entities;
        }

        public Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelated related, IQuery query)
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelated related, IQuery query)
        {
            throw new NotSupportedException();
        }

        public async Task<IEntity?> GetByIdAsync(string id, IParent? parent)
        {
            var response = await _getEntityCommandHandler.HandleAsync(new GetByIdRequest<ModelEntity>(id));

            return response.Entity;
        }

        public async Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelEntity> typedEditContext)
            {
                var response = await _insertEntityCommandHandler.HandleAsync(new InsertRequest<ModelEntity>(typedEditContext.Entity));

                return response.Entity;
            }

            return default;
        }

        public Task<IEntity> NewAsync(IParent? parent, Type? variantType)
        {
            return Task.FromResult<IEntity>(new ModelEntity());
        }

        public Task RemoveAsync(IRelated related, string id)
        {
            throw new NotSupportedException();
        }

        public Task ReorderAsync(string? beforeId, string id, IParent? parent)
        {
            throw new NotSupportedException();
        }

        public async Task UpdateAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelEntity> typedEditContext)
            {
                await _updateEntityCommandHandler.HandleAsync(new UpdateRequest<ModelEntity>(typedEditContext.Entity));
            }
        }
    }
}
