using System.Collections.Generic;
using System.Linq;
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
            var dataCollectionExpression = default(string?);

            if (property.Value<JObject>("Validations") is JObject validationsRoot &&
                validationsRoot.Value<JArray>("$values") is JArray validations)
            {
                var enabledValidations = validations
                    .OfType<JObject>()
                    .Select(x => (validationModel: x, validationConfig: x.Value<JObject>("Config")))
                    .Where(x => x.validationConfig?.Value<bool>("IsEnabled") == true);

                foreach (var (validation, validationConfig) in enabledValidations)
                {
                    if (validationConfig.Value<string>("DataCollectionExpression") is string dataCollection)
                    {
                        dataCollectionExpression = dataCollection;
                    }

                    if (validationConfig.Value<string>("RelatedCollectionAlias") is string related)
                    {
                        relatedCollectionAlias = related;
                    }

                    if (validationConfig.Value<string>("RelatedPropertyName") is string relatedPropName)
                    {
                        relatedPropertyName = relatedPropName;
                    }

                    if (validationConfig.Value<string>("ValidationMethodName") is string validationMethodName)
                    {
                        var validationInfo = new ValidationInformation(validationMethodName);

                        var configProperty = validationConfig.Properties().FirstOrDefault(x => !_defaultConfigProperties.Contains(x.Name));
                        if (configProperty.Value is JValue value &&
                            value.Value is object valueObject)
                        {
                            validationInfo.HasValue(valueObject);
                        }
                        else if (configProperty.Value is JObject @object && 
                            @object.Value<Dictionary<string, string>>() is Dictionary<string, string> dictionary)
                        {
                            validationInfo.HasDictionary(dictionary);
                        }

                        info.AddValidation(validationInfo);
                    }
                }
            }

            var relation = property.Value<bool>("IsRelationToOne") == true ? Relation.ToOne :
                property.Value<bool>("IsRelationToMany") == true ? Relation.ToMany :
                Relation.None;

            info.IsRelation(
                relation,
                relatedCollectionAlias,
                relatedPropertyName,
                dataCollectionExpression);

            return info;
        }
    }
}
