using System.Linq;
using Newtonsoft.Json.Linq;
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

            if (entity.Value<JObject>("PublishedProperties") is JObject propsRoot &&
                propsRoot.Value<JArray>("$values") is JArray properties)
            {
                foreach (var property in properties.OfType<JObject>())
                {
                    info.AddProperty(_propertyParser.ParseProperty(property));
                }
            }

            return info;
        }
    }
}
