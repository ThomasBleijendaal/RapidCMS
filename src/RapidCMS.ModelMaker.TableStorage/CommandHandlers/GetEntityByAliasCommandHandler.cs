using System.Linq;
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
    internal class GetEntityByAliasCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<GetByAliasRequest<TEntity>, EntityResponse<TEntity>>
        where TEntity : class, IModelMakerEntity
    {
        public GetEntityByAliasCommandHandler(CloudTableClient tableClient) : base(tableClient)
        {
        }

        public async Task<EntityResponse<TEntity>> HandleAsync(GetByAliasRequest<TEntity> request)
        {
            await _cloudTable.CreateIfNotExistsAsync();

            var query = _cloudTable.CreateQuery<ModelTableEntity<TEntity>>()
                .Where(x => x.PartitionKey == _partitionKey && x.Alias == request.Alias);

            return new EntityResponse<TEntity>
            {
                Entity = query.FirstOrDefault()?.Entity
            };
        }
    }
}
