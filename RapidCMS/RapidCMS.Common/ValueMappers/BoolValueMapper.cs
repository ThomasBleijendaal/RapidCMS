using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(bool))]
    public class BoolValueMapper : ValueMapper<bool>
    {
        public override bool MapFromEditor(object value)
        {
            if (value is string stringValue)
            {
                return bool.TryParse(stringValue, out var boolValue) ? boolValue : default;
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

        public override object MapToEditor(bool value)
        {
            return value;
        }
    }
}
