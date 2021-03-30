using System;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.TableStorage.Entities
{
    internal class ModelTableEntity<TEntity> : TableEntity
        where TEntity : IModelMakerEntity
    {
        public ModelTableEntity()
        {

        }

        public ModelTableEntity(TEntity entity, string partitionKey)
        {
            Entity = entity;
            PartitionKey = partitionKey;
        }

        [IgnoreProperty]
        public TEntity? Entity
        {
            get
            {
                var entity = JsonConvert.DeserializeObject<TEntity>(EntityJson ?? "");

                entity.Id = RowKey;

                return entity;
            }
            set
            {
                EntityJson = JsonConvert.SerializeObject(value);

                RowKey = value?.Id ?? Guid.NewGuid().ToString();
                Alias = value?.Alias;
            }
        }

        public string? EntityJson { get; set; }

        public string? Alias { get; set; }
    }
}
