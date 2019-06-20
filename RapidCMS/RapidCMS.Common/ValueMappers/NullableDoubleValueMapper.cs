using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(double?))]
    public class NullableDoubleValueMapper : ValueMapper<double?>
    {
        public override double? MapFromEditor(object value)
        {
            if (value is string stringValue)
            {
                return int.TryParse(stringValue, out var boolValue) ? boolValue : default(double?);
            }
            else if (value is double doubleValue)
            {
                return doubleValue;
            }
            else
            {
                return default;
            }
        }

        public override object MapToEditor(double? value)
        {
            return value;
        }
    }
}
