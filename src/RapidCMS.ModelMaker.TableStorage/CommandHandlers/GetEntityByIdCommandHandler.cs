//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Azure.Cosmos.Table;
//using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
//using RapidCMS.ModelMaker.Abstractions.Entities;
//using RapidCMS.ModelMaker.Models.Commands;
//using RapidCMS.ModelMaker.Models.Responses;
//using RapidCMS.ModelMaker.TableStorage.Abstractions;
//using RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base;
//using RapidCMS.ModelMaker.TableStorage.Entities;

//namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers
//{
//    internal class GetEntityByIdCommandHandler<TEntity> : TableClientCommandHandler<TEntity>,
//            ICommandHandler<GetByIdRequest<TEntity>, EntityResponse<TEntity>>
//        where TEntity : class, IModelMakerEntity
//    {
//        private readonly ITableEntityResolver<TEntity> _tableEntityResolver;

//        public GetEntityByIdCommandHandler(
//            ITableEntityResolver<TEntity> tableEntityResolver,
//            CloudTableClient tableClient) : base(tableClient)
//        {
//            _tableEntityResolver = tableEntityResolver;
//        }

//        public async Task<EntityResponse<TEntity>> HandleAsync(GetByIdRequest<TEntity> request)
//        {
//            await _cloudTable.CreateIfNotExistsAsync();

//            var query = _cloudTable.CreateQuery<ModelTableEntity>()
//                .Where(x => x.RowKey == request.Id);

//            if (!string.IsNullOrWhiteSpace(request.Alias))
//            {
//                query = query.Where(x => x.PartitionKey == request.Alias);
//            }
            
//            return new EntityResponse<TEntity>
//            {
//                Entity = query.Select(_tableEntityResolver.ResolveEntity).FirstOrDefault()
//            };
//        }
//    }
//}
