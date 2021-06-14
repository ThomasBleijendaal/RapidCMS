//using Microsoft.Azure.Cosmos.Table;
//using RapidCMS.ModelMaker.Abstractions.Entities;
//using RapidCMS.ModelMaker.TableStorage.Extensions;

//namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base
//{
//    internal abstract class TableClientCommandHandler<TEntity>
//        where TEntity : IModelMakerEntity
//    {
//        protected readonly CloudTable _cloudTable;

//        public TableClientCommandHandler(CloudTableClient tableClient)
//        {
//            _cloudTable = tableClient.GetTableReference(typeof(TEntity).GetTableName());
//        }
//    }
//}
