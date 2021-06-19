using System.Linq;
using Newtonsoft.Json.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Parsers
{
    internal sealed class PropertyParser
    {
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
                    if (validation.Value<string>("AttributeExpression") is string attribute)
                    {
                        info.HasValidationAttribute(attribute);
                    }

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
                }
            }

            if (property.Value<bool>("IsRelationToOne") is bool isRelationToOne &&
                property.Value<bool>("IsRelationToMany") is bool isRelationToMany)
            {
                info.IsRelation(
                    isRelationToOne ? Relation.ToOne : isRelationToMany ? Relation.ToMany : Relation.None,
                    relatedCollectionAlias,
                    relatedPropertyName,
                    dataCollectionExpression);
            }

            return info;
        }
    }
}
