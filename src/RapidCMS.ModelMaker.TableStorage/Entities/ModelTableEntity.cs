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

        public ModelTableEntity(TEntity entity)
        {
            Entity = entity;
        }

        [IgnoreProperty]
        public TEntity? Entity
        {
            get => JsonConvert.DeserializeObject<TEntity>(EntityJson ?? "");
            set => JsonConvert.SerializeObject(Entity);
        }

        public string? EntityJson { get; set; }

        public string? Alias { get; set; }
    }
}
