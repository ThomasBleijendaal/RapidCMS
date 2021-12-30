using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Parsers
{
    internal sealed class PropertyParser
    {
        private readonly string[] _defaultConfigProperties = new[]
        {
            "IsEnabled",
            "AlwaysIncluded",
            "RelatedCollectionAlias",
            "DataCollectionExpression",
            "ValidiationMethodName",
            "$type"
        };

        public PropertyInformation ParseProperty(EntityInformation entity, JObject property)
        {
            var info = new PropertyInformation();

            if (property.Value<string>("Name") is string propertyName)
            {
                info.HasName(propertyName);
            }

            if (property.Value<string>("Type") is string propertyType)
            {
                info.IsType(propertyType);
            }

            if (property.Value<string>("EditorType") is string editorType)
            {
                info.UsesEditor(editorType);
            }

            if (property.Value<bool>("IsRequired") is bool required)
            {
                info.IsRequired(required);
            }

            if (property.Value<bool>("IsTitle") is bool title)
            {
                info.IsTitle(title);
            }

            if (property.Value<bool>("IncludeInListView") is bool include)
            {
                info.ShouldBeDisplayedInListView(include);
            }

            var relatedCollectionAlias = default(string?);
            var relatedPropertyName = default(string?);

            if (property.Value<JObject>("Details") is JObject detailsRoot &&
                detailsRoot.Value<JArray>("$values") is JArray details)
            {
                var enabledDetails = details
                    .OfType<JObject>()
                    .Select(x => (validationModel: x, validationConfig: x.Value<JObject>("Config")))
                    .Where(x => x.validationConfig?.Value<bool>("IsEnabled") == true);

                foreach (var (detail, detailConfig) in enabledDetails)
                {
                    if (ParseType(detailConfig.Value<string>("$type")) is not (string @namespace, string typeName))
                    {
                        continue;
                    }

                    var detailInfo = new PropertyDetailInformation(typeName, @namespace);

                    if (detailConfig.Value<string>("DataCollectionType") is string dataCollection &&
                        ParseTypeWithNamespace(dataCollection) is string dataCollectionType)
                    {
                        detailInfo.HasDataCollection(dataCollectionType);
                    }

                    if (detailConfig.Value<string>("RelatedCollectionAlias") is string related)
                    {
                        relatedCollectionAlias = related;
                    }

                    if (detailConfig.Value<string>("RelatedPropertyName") is string relatedPropName)
                    {
                        relatedPropertyName = relatedPropName;
                    }

                    if (detailConfig.Value<string>("ValidationMethodName") is string validationMethodName)
                    {
                        detailInfo.HasValidationMethod(validationMethodName);
                    }

                    var configProperty = detailConfig.Properties().FirstOrDefault(x => !_defaultConfigProperties.Contains(x.Name));

                    if (configProperty?.Value is JValue value &&
                        value.Value is object valueObject)
                    {
                        detailInfo.HasConfigValue(valueObject);
                    }
                    else if (configProperty?.Value is JObject listObject &&
                        listObject.ContainsKey("$values") &&
                        listObject.Value<JArray>("$values") is JArray array &&
                        array.Values<string>() is IEnumerable<string> list)
                    {
                        detailInfo.HasConfigList(configProperty.Name, list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList()!);
                    }
                    else if (configProperty?.Value is JObject configObject &&
                        configObject.ContainsKey("$type"))
                    {
                        if (ParseType(configObject.Value<string>("$type")) is not (string configNamespace, string configType))
                        {
                            continue;
                        }

                        var dictionary = configObject.Properties()
                            .Where(x => x.Name != "$type")
                            .Where(x => x.Value is JValue value && value.Value is object valueObject)
                            .ToDictionary(x => x.Name, x => ((JValue)x.Value).Value!);

                        detailInfo.HasSubClass(configProperty.Name, configType, configNamespace, dictionary);
                    }

                    info.AddDetail(detailInfo);
                }
            }

            var relation = property.Value<bool>("IsRelationToOne") == true ? Relation.ToOne :
                property.Value<bool>("IsRelationToMany") == true ? Relation.ToMany :
                Relation.None;

            info.IsRelation(
                relation,
                relatedCollectionAlias,
                relatedPropertyName);

            return info;
        }

        public string? ParseTypeWithNamespace(string? type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return default;
            }

            var replacedType = Regex.Replace(Regex.Replace(type!
                    .Replace("[[", "<")
                    .Replace("]]", ">")
                    .Replace("],[", ", "),
                "`[0-9]+",
                ""),
                @"\,\s[a-zA-Z-0-9.]+\,\sVersion=[a-zA-Z0-9.]+\,\sCulture=[a-zA-Z0-9]+\,\sPublicKeyToken=[a-zA-Z0-9]+",
                "");

            return replacedType;
        }

        private (string? @namespace, string? typeName) ParseType(string? type)
        {
            var parts = type?.Split(',').FirstOrDefault()?.Split('.').ToList();
            if (parts == null)
            {
                return default;
            }

            var typeName = parts.Last().Replace("+", ".");
            var @namespace = parts.Count < 2 ? string.Empty : string.Join(".", parts.Take(parts.Count - 1));

            return (@namespace, typeName);
        }
    }
}
