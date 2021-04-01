using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.TableStorage.Abstractions;
using RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base;

namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers
{
    internal class UpdateEntityCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<UpdateRequest<TEntity>, ConfirmResponse>
        where TEntity : class, IModelMakerEntity
    {
        private readonly ITableEntityResolver<TEntity> _tableEntityResolver;

        public UpdateEntityCommandHandler(
            ITableEntityResolver<TEntity> tableEntityResolver,
            CloudTableClient tableClient) : base(tableClient)
        {
            _tableEntityResolver = tableEntityResolver;
        }

        public async Task<ConfirmResponse> HandleAsync(UpdateRequest<TEntity> request)
        {
            var existingEntity = _tableEntityResolver.ResolveTableEntity(request.Entity, _partitionKey);

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
