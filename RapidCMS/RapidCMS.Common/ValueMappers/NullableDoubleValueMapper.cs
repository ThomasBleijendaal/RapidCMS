using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(double?))]
    public class NullableDoubleValueMapper : ValueMapper<double?>
    {
        public override double? MapFromEditor(ValueMappingContext context, object value)
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

        public override object MapToEditor(ValueMappingContext context, double? value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, double? value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
