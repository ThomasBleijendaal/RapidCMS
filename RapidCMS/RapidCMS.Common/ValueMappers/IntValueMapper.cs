using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(int))]
    public class IntValueMapper : ValueMapper<int>
    {
        public override int MapFromEditor(ValueMappingContext context, object value)
        {
            if (value is string stringValue)
            {
                return int.TryParse(stringValue, out var boolValue) ? boolValue : default;
            }
            else if (value is int intValue)
            {
                return intValue;
            }
            else
            {
                return default;
            }
        }

        public override object MapToEditor(ValueMappingContext context, int value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, int value)
        {
            return value.ToString();
        }
    }
}
