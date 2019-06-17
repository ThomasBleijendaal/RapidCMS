using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(long))]
    public class LongValueMapper : ValueMapper<long>
    {
        public override long MapFromEditor(ValueMappingContext context, object value)
        {
            if (value is string stringValue)
            {
                return long.TryParse(stringValue, out var longValue) ? longValue : default;
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

        public override object MapToEditor(ValueMappingContext context, long value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, long value)
        {
            return value.ToString();
        }
    }
}
