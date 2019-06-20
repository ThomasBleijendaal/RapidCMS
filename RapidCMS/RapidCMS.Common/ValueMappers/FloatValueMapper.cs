using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(float))]
    public class FloatValueMapper : ValueMapper<float>
    {
        public override float MapFromEditor(object value)
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

        public override object MapToEditor(float value)
        {
            return value;
        }
    }
}
