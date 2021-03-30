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
    internal class InsertEntityCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<InsertRequest<TEntity>, EntityResponse<TEntity>>
        where TEntity : class, IModelMakerEntity
    {
        public InsertEntityCommandHandler(CloudTableClient tableClient) : base(tableClient)
        {
        }

        public async Task<EntityResponse<TEntity>> HandleAsync(InsertRequest<TEntity> request)
        {
            var newEntity = new ModelTableEntity<TEntity>(request.Entity, _partitionKey);

            var insert = TableOperation.Insert(newEntity);

            var result = await _cloudTable.ExecuteAsync(insert);

            return new EntityResponse<TEntity>
            {
                Entity = (result.Result as ModelTableEntity<TEntity>)?.Entity
            };
        }
    }
}
