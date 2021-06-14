//using System;
//using Newtonsoft.Json;
//using RapidCMS.ModelMaker.Models.Entities;
//using RapidCMS.ModelMaker.TableStorage.Abstractions;
//using RapidCMS.ModelMaker.TableStorage.Entities;

//namespace RapidCMS.ModelMaker.TableStorage.Resolvers
//{
//    internal class ModelMakerEntityResolver : ITableEntityResolver<ModelMakerEntity>
//    {
//        private readonly JsonSerializerSettings _settings;

//        public ModelMakerEntityResolver()
//        {
//            _settings = new JsonSerializerSettings
//            {
//                // TODO: is this the best way?
//                TypeNameHandling = TypeNameHandling.All
//            };
//        }

//        public ModelMakerEntity? ResolveEntity(ModelTableEntity tableEntity)
//        {
//            // TODO: what if the class has changed and the json cannot fit it anymore?
//            return JsonConvert.DeserializeObject<ModelMakerEntity>(tableEntity.EntityJson ?? "", _settings);
//        }

//        public ModelTableEntity ResolveTableEntity(ModelMakerEntity entity)
//        {
//            var id = entity.Id ?? Guid.NewGuid().ToString();
//            entity.Id ??= id;

//            return new ModelTableEntity
//            {
//                EntityJson = JsonConvert.SerializeObject(entity, _settings),
//                PartitionKey = entity.Alias,
//                RowKey = id
//            };
//        }
//    }
//}
