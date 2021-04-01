using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.TableStorage.Abstractions;
using RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base;
using RapidCMS.ModelMaker.TableStorage.Entities;

namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers
{
    internal class GetAllEntitiesCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<GetAllRequest<TEntity>, EntitiesResponse<TEntity>>
        where TEntity : class, IModelMakerEntity
    {
        private readonly ITableEntityResolver<TEntity> _tableEntityResolver;

        public GetAllEntitiesCommandHandler(
            ITableEntityResolver<TEntity> tableEntityResolver,
            CloudTableClient tableClient) : base(tableClient)
        {
            _tableEntityResolver = tableEntityResolver;
        }

        public async Task<EntitiesResponse<TEntity>> HandleAsync(GetAllRequest<TEntity> request)
        {
            await _cloudTable.CreateIfNotExistsAsync();

            var query = _cloudTable.CreateQuery<ModelTableEntity>()
                .Where(x => x.PartitionKey == _partitionKey);

            var data = query.ToList();

            return new EntitiesResponse<TEntity>
            {
                Entities = data.SelectNotNull(_tableEntityResolver.ResolveEntity)
            };
        }
    }
}
