using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.TableStorage.Abstractions;
using RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base;
using RapidCMS.ModelMaker.TableStorage.Entities;

namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers
{
    internal class InsertEntityCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
            ICommandHandler<InsertRequest<TEntity>, EntityResponse<TEntity>>
        where TEntity : class, IModelMakerEntity
    {
        private readonly ITableEntityResolver<TEntity> _tableEntityResolver;

        public InsertEntityCommandHandler(
            ITableEntityResolver<TEntity> tableEntityResolver, 
            CloudTableClient tableClient) : base(tableClient)
        {
            _tableEntityResolver = tableEntityResolver;
        }

        public async Task<EntityResponse<TEntity>> HandleAsync(InsertRequest<TEntity> request)
        {
            var newEntity = _tableEntityResolver.ResolveTableEntity(request.Entity);

            var insert = TableOperation.Insert(newEntity);

            var data = await _cloudTable.ExecuteAsync(insert);

            return new EntityResponse<TEntity>
            {
                Entity = (data.Result is ModelTableEntity tableEntity)
                    ? _tableEntityResolver.ResolveEntity(tableEntity)
                    : default
            };
        }
    }
}
