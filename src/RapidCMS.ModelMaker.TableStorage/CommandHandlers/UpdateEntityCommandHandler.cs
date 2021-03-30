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
    internal class UpdateEntityCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<UpdateRequest<TEntity>, ConfirmResponse>
        where TEntity : class, IModelMakerEntity
    {
        public UpdateEntityCommandHandler(CloudTableClient tableClient) : base(tableClient)
        {
        }

        public async Task<ConfirmResponse> HandleAsync(UpdateRequest<TEntity> request)
        {
            var existingEntity = new ModelTableEntity<TEntity>(request.Entity, _partitionKey);

            var replace = TableOperation.Replace(existingEntity);

            var result = await _cloudTable.ExecuteAsync(replace);

            return new ConfirmResponse
            {
                Success = result.Result != null
            };
        }
    }
}
