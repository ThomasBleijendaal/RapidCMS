using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base;
using RapidCMS.ModelMaker.TableStorage.Entities;

namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers
{
    internal class GetEntityByIdCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<GetByIdRequest<TEntity>, EntityResponse<TEntity>>
        where TEntity : class, IModelMakerEntity
    {
        public GetEntityByIdCommandHandler(CloudTableClient tableClient) : base(tableClient)
        {
        }

        public async Task<EntityResponse<TEntity>> HandleAsync(GetByIdRequest<TEntity> request)
        {
            var fetch = TableOperation.Retrieve<ModelTableEntity<TEntity>>(_partitionKey, request.Id);

            var data = await _cloudTable.ExecuteAsync(fetch);

            return new EntityResponse<TEntity>
            {
                Entity = data.Result as TEntity
            };
        }
    }
}
