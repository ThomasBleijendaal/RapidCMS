using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    public class LongValueMapper : ValueMapper<long>
    {
        public override long MapFromEditor(ValueMappingContext context, object value)
        {
            return (long)value;
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
