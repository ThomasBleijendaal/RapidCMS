using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    public class LongValueMapper : ValueMapper<long?>
    {
        public override long? MapFromEditor(ValueMappingContext context, object value)
        {
            return value == null ? default(long?) : (long)value;
        }

        public override object MapToEditor(ValueMappingContext context, long? value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, long? value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
