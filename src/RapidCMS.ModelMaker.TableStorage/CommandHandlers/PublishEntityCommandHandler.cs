using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.TableStorage.Abstractions;
using RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base;

namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers
{
    internal class PublishEntityCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<PublishRequest<TEntity>, ConfirmResponse>
        where TEntity : class, IModelMakerEntity
    {
        private readonly ITableEntityResolver<TEntity> _tableEntityResolver;
        private readonly ICommandHandler<GetByIdRequest<TEntity>, EntityResponse<TEntity>> _commandHandler;

        public PublishEntityCommandHandler(
            ITableEntityResolver<TEntity> tableEntityResolver,
           ICommandHandler<GetByIdRequest<TEntity>, EntityResponse<TEntity>> commandHandler,
            CloudTableClient tableClient) : base(tableClient)
        {
            _tableEntityResolver = tableEntityResolver;
            _commandHandler = commandHandler;
        }

        public async Task<ConfirmResponse> HandleAsync(PublishRequest<TEntity> request)
        {
            if (string.IsNullOrEmpty(request.Entity.Id))
            {
                throw new InvalidOperationException();
            }

            // TODO: check if refreshing of the entity can be done by the CMS via IMediator events
            // TODO: this causes bugs
            //var entity = (await _commandHandler.HandleAsync(new GetByIdRequest<TEntity>(request.Entity.Id!, request.Entity.Alias)).ConfigureAwait(false)).Entity;
            //if (entity == null)
            //{
            //    throw new InvalidOperationException();
            //}

            // TODO: move publish logic to some external resolver + check how publishing should be supported model maker wide
            if (request.Entity is ModelEntity modelEntity)
            {
                modelEntity.PublishedProperties = modelEntity.DraftProperties;
            }

            var existingEntity = _tableEntityResolver.ResolveTableEntity(request.Entity);

            // TODO: always *?
            existingEntity.ETag = "*";

            var replace = TableOperation.Replace(existingEntity);

            var result = await _cloudTable.ExecuteAsync(replace);

            return new ConfirmResponse
            {
                Success = result.Result != null
            };
        }
    }
}
