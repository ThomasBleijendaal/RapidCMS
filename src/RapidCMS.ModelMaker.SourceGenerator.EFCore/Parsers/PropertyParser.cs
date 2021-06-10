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

            return info;
        }
    }
}
