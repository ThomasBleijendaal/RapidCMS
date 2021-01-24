using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.ApiBridge;

namespace RapidCMS.Core.Converters
{
    public class EntityModelJsonConverter<TEntity> : JsonConverter<EntityModel<TEntity>>
        where TEntity : class, IEntity
    {
        private readonly IReadOnlyDictionary<string, Type> _typeDictionary;

        public EntityModelJsonConverter()
        {
            var derivedTypes = typeof(TEntity).Assembly
                .GetTypes()
                .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(TEntity)));

            _typeDictionary = derivedTypes.Append(typeof(TEntity)).ToDictionary(x => AliasHelper.GetEntityVariantAlias(x));
        }

        public override bool CanRead => true;

        public override EntityModel<TEntity> ReadJson(JsonReader reader, Type objectType, EntityModel<TEntity>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var property = obj?.Property(nameof(EntityModel<TEntity>.VariantAlias), StringComparison.InvariantCultureIgnoreCase);
            var alias = property?.Value.ToString();

            if (obj != null && alias != null && _typeDictionary.TryGetValue(alias, out var type))
            {
                var entityObject = obj.Property(nameof(EntityModel<TEntity>.Entity), StringComparison.InvariantCultureIgnoreCase)?.Value;
                if (entityObject?.ToObject(type) is TEntity entity)
                {
                    return EntityModel.Create(entity);
                }
            }

            throw new InvalidOperationException($"Invalid json for {nameof(EntityModel<TEntity>)} given.");
        }


        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, EntityModel<TEntity>? value, JsonSerializer serializer)
        {
            throw new NotImplementedException("The default serializer can do this perfectly well.");
        }
    }
}
