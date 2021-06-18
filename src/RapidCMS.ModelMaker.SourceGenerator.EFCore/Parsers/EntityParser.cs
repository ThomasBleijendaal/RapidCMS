using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Contexts;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Parsers
{
    internal sealed class EntityParser
    {
        private readonly PropertyParser _propertyParser;

        public EntityParser(PropertyParser propertyParser)
        {
            _propertyParser = propertyParser;
        }

        public EntityInformation ParseEntity(JObject entity)
        {
            var info = new EntityInformation();

            if (entity.Value<string>("Name") is string entityName)
            {
                info.HasName(entityName);
            }

            if (entity.Value<string>("Alias") is string alias)
            {
                info.HasAlias(alias);
            }

            if (entity.Value<string>("Icon") is string icon)
            {
                info.HasIcon(icon, entity.Value<string?>("IconColor"));
            }

            if (entity.Value<string>("Output") is string outputItems)
            {
                info.ShouldOutput(outputItems.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }

            if (entity.Value<JObject>("Properties") is JObject propsRoot &&
                propsRoot.Value<JArray>("$values") is JArray properties)
            {
                foreach (var property in properties.OfType<JObject>())
                {
                    info.AddProperty(_propertyParser.ParseProperty(property));
                }
            }

            return info;
        }

        public void ProcessEntity(EntityInformation info, ModelMakerContext context)
        {
            var oneWayRelationsToThisEntity = context.Entities
                .Where(entity => entity != info)
                .SelectMany(entity => entity.Properties
                    .Where(x => (x.RelatedToOneEntity || x.RelatedToManyEntities) &&
                        x.Type == $"{context.Namespace}.{info.Name}" &&
                        !info.Properties.Any(x => x.Type == $"{context.Namespace}.{entity.Name}"))
                    .Select(property => new { Entity = entity, Property = property }))
                .ToList();

            foreach (var relation in oneWayRelationsToThisEntity)
            {
                var adHocProperty = new PropertyInformation(true)
                    .UseFor(Use.Entity)
                    .HasName($"{relation.Entity.Name}{relation.Property.Name}")
                    .IsType($"{context.Namespace}.{relation.Entity.Name}")
                    .IsRelation(false, true, relation.Property.RelatedCollectionAlias, default);

                info.AddProperty(adHocProperty);
            }
        }
    }
}
