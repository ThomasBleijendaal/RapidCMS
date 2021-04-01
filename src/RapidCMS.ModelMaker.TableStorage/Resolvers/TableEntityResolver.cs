using System;
using Newtonsoft.Json;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.TableStorage.Abstractions;
using RapidCMS.ModelMaker.TableStorage.Entities;

namespace RapidCMS.ModelMaker.TableStorage.Resolvers
{
    internal class TableEntityResolver<TEntity> : ITableEntityResolver<TEntity>
        where TEntity : IModelMakerEntity
    {
        private readonly JsonSerializerSettings _settings;

        public TableEntityResolver()
        {
            _settings = new JsonSerializerSettings
            {
                // TODO: is this the best way?
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public TEntity? ResolveEntity(ModelTableEntity tableEntity)
        {
            // TODO: what if the class has changed and the json cannot fit it anymore?
            return JsonConvert.DeserializeObject<TEntity>(tableEntity.EntityJson ?? "", _settings);
        }

        public ModelTableEntity ResolveTableEntity(TEntity entity, string partitionKey)
        {
            var id = entity.Id ?? Guid.NewGuid().ToString();
            entity.Id ??= id;

            return new ModelTableEntity
            {
                Alias = entity.Alias,
                EntityJson = JsonConvert.SerializeObject(entity, _settings),
                PartitionKey = partitionKey,
                RowKey = id
            };
        }
    }
}
