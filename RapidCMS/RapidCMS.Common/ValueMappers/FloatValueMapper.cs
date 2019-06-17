using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(float))]
    public class FloatValueMapper : ValueMapper<float>
    {
        public override float MapFromEditor(ValueMappingContext context, object value)
        {
            if (value is string stringValue)
            {
                return float.TryParse(stringValue, out var boolValue) ? boolValue : default;
            }
            else if (value is float floatValue)
            {
                return floatValue;
            }
            else
            {
                return default;
            }
        }

        public override object MapToEditor(ValueMappingContext context, float value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, float value)
        {
            return value.ToString();
        }
    }
}
