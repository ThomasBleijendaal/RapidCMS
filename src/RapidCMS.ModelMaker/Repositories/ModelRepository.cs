using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Extenstions;
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
        private readonly ICommandHandler<PublishRequest<ModelEntity>, ConfirmResponse> _publishEntityCommandHandler;

        public ModelRepository(
            ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse> removeCommandHandler,
            ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>> getAllEntitiesCommandHandler,
            ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> getEntityCommandHandler,
            ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>> insertEntityCommandHandler,
            ICommandHandler<PublishRequest<ModelEntity>, ConfirmResponse> publishEntityCommandHandler)
        {
            _removeCommandHandler = removeCommandHandler;
            _getAllEntitiesCommandHandler = getAllEntitiesCommandHandler;
            _getEntityCommandHandler = getEntityCommandHandler;
            _insertEntityCommandHandler = insertEntityCommandHandler;
            _publishEntityCommandHandler = publishEntityCommandHandler;
        }

        public Task AddAsync(IRelatedViewContext viewContext, string id) => throw new NotSupportedException();

        public async Task DeleteAsync(string id, IViewContext viewContext) => await _removeCommandHandler.HandleAsync(new RemoveRequest<ModelEntity>(id, id));

        public async Task<IEnumerable<IEntity>> GetAllAsync(IViewContext viewContext, IQuery query)
        {
            var response = await _getAllEntitiesCommandHandler.HandleAsync(new GetAllRequest<ModelEntity>());

            return response.Entities;
        }

        public Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelatedViewContext viewContext, IQuery query) => throw new NotSupportedException();

        public Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelatedViewContext viewContext, IQuery query) => throw new NotSupportedException();

        public async Task<IEntity?> GetByIdAsync(string id, IViewContext viewContext)
        {
            var response = await _getEntityCommandHandler.HandleAsync(new GetByIdRequest<ModelEntity>(id));

            return response.Entity;
        }

        public async Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelEntity> typedEditContext)
            {
                var entity = typedEditContext.Entity;
                entity.Alias = entity.Name.ToUrlFriendlyString(); // TODO: ensure uniqueness

                var response = await _insertEntityCommandHandler.HandleAsync(new InsertRequest<ModelEntity>(entity));

                return response.Entity;
            }

            return default;
        }

        public Task<IEntity> NewAsync(IViewContext viewContext, Type? variantType) => Task.FromResult<IEntity>(new ModelEntity());

        public Task RemoveAsync(IRelatedViewContext viewContext, string id) => throw new NotSupportedException();

        public Task ReorderAsync(string? beforeId, string id, IViewContext viewContext) => throw new NotSupportedException();

        public async Task UpdateAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelEntity> typedEditContext)
            {
                await _publishEntityCommandHandler.HandleAsync(new PublishRequest<ModelEntity>(typedEditContext.Entity));
            }
        }
    }
}
