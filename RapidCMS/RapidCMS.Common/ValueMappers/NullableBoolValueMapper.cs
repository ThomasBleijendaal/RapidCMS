using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(bool?))]
    public class NullableBoolValueMapper : ValueMapper<bool?>
    {
        public override bool? MapFromEditor(ValueMappingContext context, object value)
        {
            if (value is string stringValue)
            {
                return bool.TryParse(stringValue, out var boolValue) ? boolValue : default(bool?);
            }
            else if (value is bool boolValue)
            {
                return boolValue;
            }
            else
            {
                return default;
            }
        }

        public override object MapToEditor(ValueMappingContext context, bool? value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, bool? value)
        {
            return value?.ToString() ?? "Null";
        }
    }
}
