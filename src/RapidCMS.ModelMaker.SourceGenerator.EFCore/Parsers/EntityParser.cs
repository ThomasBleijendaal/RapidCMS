﻿using System;
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
                info.HasName(entityName, entity.Value<string>("PluralName"));
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
                    info.AddProperty(_propertyParser.ParseProperty(info, property));
                }
            }

            return info;
        }

        public void NormalizeEntity(EntityInformation info, ModelMakerContext context)
        {
            // both sides of the relation are configured with property names
            var twoWayRelationsToThisEntity = context.Entities
                .Where(entity => entity != info)
                .SelectMany(entity => entity.Properties
                    .Where(x => x.Relation != Relation.None &&
                        x.Type == $"{context.Namespace}.{info.PascalName}" &&
                        info.Properties.Any(p => p.PascalName == x.RelatedPropertyName))
                    .Select(property => new { Entity = entity, Property = property }))
                .ToList();

            foreach (var relation in twoWayRelationsToThisEntity)
            {
                try
                {
                    var property = info.Properties.Single(x => x.PascalName == relation.Property.RelatedPropertyName);

                    var value = relation.Property.Relation & ~(Relation.One | Relation.Many);

                    property.Relation |= value switch
                    {
                        Relation.ToOne => Relation.One,
                        Relation.ToMany => Relation.Many,
                        _ => Relation.None
                    };
                }
                catch
                {
                    continue;
                }
            }

            // only one side of the relation is configured with property names
            var oneWayRelationsToThisEntity = context.Entities
                .Where(entity => entity != info)
                .SelectMany(entity => entity.Properties
                    .Where(x => x.Relation != Relation.None &&
                        x.Type == $"{context.Namespace}.{info.PascalName}" &&
                        string.IsNullOrEmpty(x.RelatedPropertyName))
                    .Select(property => new { Entity = entity, Property = property }))
                .ToList();

            foreach (var relation in oneWayRelationsToThisEntity)
            {
                try
                {
                    var reciprocalProperty = info.Properties.SingleOrDefault(x => x.RelatedPropertyName == relation.Property.PascalName);
                    if (reciprocalProperty != null)
                    {
                        relation.Property.RelatedPropertyName = reciprocalProperty.PascalName;

                        var value = relation.Property.Relation & ~(Relation.One | Relation.Many);

                        reciprocalProperty.Relation |= value switch
                        {
                            Relation.ToOne => Relation.One,
                            Relation.ToMany => Relation.Many,
                            _ => Relation.None
                        };
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        public void ExtendEntity(EntityInformation info, ModelMakerContext context)
        {
            // only one of the entities of this relation is configured
            var oneWayRelationsToThisEntity = context.Entities
                .Where(entity => entity != info)
                .SelectMany(entity => entity.Properties
                    .Where(x => x.Relation != Relation.None &&
                        x.Type == $"{context.Namespace}.{info.PascalName}" &&
                        string.IsNullOrEmpty(x.RelatedPropertyName))
                    .Select(property => new { Entity = entity, Property = property }))
                .ToList();

            foreach (var relation in oneWayRelationsToThisEntity)
            {
                var reverseRelation = relation.Property.Relation switch
                {
                    Relation.ToOne => Relation.One | Relation.ToMany,
                    Relation.ToMany => Relation.Many | Relation.ToMany,
                    _ => Relation.None
                };

                var adHocProperty = new PropertyInformation(true)
                    .UseFor(Use.Entity)
                    .HasName($"{relation.Entity.PascalName}{relation.Property.PascalName}")
                    .IsType($"{context.Namespace}.{relation.Entity.PascalName}")
                    .IsRelation(reverseRelation, relation.Property.RelatedCollectionAlias, $"{relation.Entity.PascalName}{relation.Property.PascalName}", default);

                relation.Property.RelatedPropertyName ??= $"{relation.Entity.PascalName}{relation.Property.PascalName}";
                relation.Property.Relation |= Relation.Many;

                info.AddProperty(adHocProperty);
            }
        }
    }
}
