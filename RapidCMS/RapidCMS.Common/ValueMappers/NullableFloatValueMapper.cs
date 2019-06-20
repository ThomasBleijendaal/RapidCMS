using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(float?))]
    public class NullableFloatValueMapper : ValueMapper<float?>
    {
        public override float? MapFromEditor(object value)
        {
            if (value is string stringValue)
            {
                return int.TryParse(stringValue, out var boolValue) ? boolValue : default(float?);
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

        public override object MapToEditor(float? value)
        {
            return value;
        }
    }
}
