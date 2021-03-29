using System;
using Microsoft.Azure.Cosmos.Table;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.TableStorage.Extensions;

namespace RapidCMS.ModelMaker.TableStorage.CommandHandlers.Base
{
    internal abstract class TableClientCommandHandler<TEntity>
        where TEntity : IModelMakerEntity
    {
        protected readonly CloudTable _cloudTable;
        protected readonly string _partitionKey = typeof(TEntity).GetPartitionKey();

        public TableClientCommandHandler(CloudTableClient tableClient)
        {
            _cloudTable = tableClient.GetTableReference(typeof(TEntity).GetTableName());
        }

        protected string GetRowKey(IModelMakerEntity modelMakerEntity)
            => modelMakerEntity.Id ?? Guid.NewGuid().ToString();
    }
}
