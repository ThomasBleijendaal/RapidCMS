using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(long?))]
    public class NullableLongValueMapper : ValueMapper<long?>
    {
        public override long? MapFromEditor(object value)
        {
            if (value is string stringValue)
            {
                return long.TryParse(stringValue, out var longValue) ? longValue : default(long?);
            }
            else if (value is long longValue)
            {
                return longValue;
            }
            else
            {
                return default;
            }
        }

        public override object MapToEditor(long? value)
        {
            return value;
        }
    }
}
