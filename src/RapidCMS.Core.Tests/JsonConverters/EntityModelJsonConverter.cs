using Newtonsoft.Json;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Converters;
using RapidCMS.Core.Models.ApiBridge;

namespace RapidCMS.Core.Tests.JsonConverters
{
    public class EntityModelJsonConverter
    {
        private JsonSerializerSettings _jsonSerializerSettings;

        [SetUp]
        public void Setup()
        {
            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.Converters.Add(new EntityModelJsonConverter<Entity>());
        }

        [Test]
        public void WhenEntityIsGivenToJsonConverter_EntityIsDeserialized()
        {
            var model = EntityModel.Create(new Entity());

            var json = JsonConvert.SerializeObject(model, _jsonSerializerSettings);

            var deserializedModel = JsonConvert.DeserializeObject<EntityModel<Entity>>(json, _jsonSerializerSettings);

            Assert.AreEqual(model.Entity.GetType(), deserializedModel.Entity.GetType());
            Assert.AreEqual(model.VariantAlias, deserializedModel.VariantAlias);
        }

        [Test]
        public void WhenSubEntityIsGivenToJsonConverter_SubEntityIsDeserialized()
        {
            var model = EntityModel.Create(new SubEntity());

            var json = JsonConvert.SerializeObject(model, _jsonSerializerSettings);

            var deserializedModel = JsonConvert.DeserializeObject<EntityModel<Entity>>(json, _jsonSerializerSettings);

            Assert.AreEqual(model.Entity.GetType(), deserializedModel.Entity.GetType());
            Assert.AreEqual(model.VariantAlias, deserializedModel.VariantAlias);
        }

        class Entity : IEntity
        {
            public string Id { get; set; }
        }

        class SubEntity : Entity
        {
        }
    }
}
