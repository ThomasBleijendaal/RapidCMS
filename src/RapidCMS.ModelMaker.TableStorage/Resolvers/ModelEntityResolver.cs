using Newtonsoft.Json;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.TableStorage.Abstractions;
using RapidCMS.ModelMaker.TableStorage.Entities;

namespace RapidCMS.ModelMaker.TableStorage.Resolvers
{
    internal class ModelEntityResolver : ITableEntityResolver<ModelEntity>
    {
        private readonly JsonSerializerSettings _settings;

        public ModelEntityResolver()
        {
            _settings = new JsonSerializerSettings
            {
                // TODO: is this the best way?
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public ModelEntity? ResolveEntity(ModelTableEntity tableEntity)
        {
            // TODO: what if the class has changed and the json cannot fit it anymore?
            return JsonConvert.DeserializeObject<ModelEntity>(tableEntity.EntityJson ?? "", _settings);
        }

        public ModelTableEntity ResolveTableEntity(ModelEntity entity)
        {
            return new ModelTableEntity
            {
                EntityJson = JsonConvert.SerializeObject(entity, _settings),
                PartitionKey = entity.Alias,
                RowKey = entity.Id
            };
        }
    }
}
