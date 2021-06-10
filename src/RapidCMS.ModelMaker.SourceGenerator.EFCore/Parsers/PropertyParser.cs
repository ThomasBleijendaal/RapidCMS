using System.Linq;
using Newtonsoft.Json.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Parsers
{
    internal sealed class PropertyParser
    {
        public PropertyInformation ParseProperty(JObject property)
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

            if (property.Value<bool>("IsRequired") is bool required)
            {
                info.IsRequired(required);
            }

            if (property.Value<bool>("IsRelationToOne") is bool isRelationToOne &&
                property.Value<bool>("IsRelationToMany") is bool isRelationToMany)
            {
                info.IsRelation(isRelationToOne, isRelationToMany);
            }

            if (property.Value<JObject>("Validations") is JObject validationsRoot &&
                validationsRoot.Value<JArray>("$values") is JArray validations)
            {
                var enabledValidations = validations
                    .OfType<JObject>()
                    .Where(x => x.Value<JObject>("Config")?.Value<bool>("IsEnabled") == true);

                foreach (var validation in enabledValidations)
                {
                    if (validation.Value<string>("Attribute") is string attribute)
                    {
                        info.HasValidationAttribute(attribute);
                    }
                }
            }

            return info;
        }
    }
}
