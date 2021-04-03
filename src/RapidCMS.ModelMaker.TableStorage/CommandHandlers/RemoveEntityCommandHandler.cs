using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base;

namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers
{
    internal class RemoveEntityCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<RemoveRequest<TEntity>, ConfirmResponse>
        where TEntity : IModelMakerEntity
    {
        public RemoveEntityCommandHandler(CloudTableClient tableClient) : base(tableClient)
        {
        }

        public async Task<ConfirmResponse> HandleAsync(RemoveRequest<TEntity> request)
        {
            var fetch = TableOperation.Retrieve(request.Alias ?? _cloudTable.Name, request.Id);

            var entity = await _cloudTable.ExecuteAsync(fetch);

            if (entity.Result is ITableEntity tableEntity)
            {
                await _cloudTable.ExecuteAsync(TableOperation.Delete(tableEntity));

                return new ConfirmResponse
                {
                    Success = true
                };
            }

            return new ConfirmResponse
            {
                Success = true
            };
        }
    }
}
