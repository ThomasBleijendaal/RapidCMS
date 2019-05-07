using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    public class DefaultValueMapper : ValueMapper<object>
    {
        public override object MapFromEditor(ValueMappingContext context, object value)
        {
            return value;
        }

        public override object MapToEditor(ValueMappingContext context, object value)
        {
            return value?.ToString() ?? string.Empty;
        }

        public override string MapToView(ValueMappingContext context, object value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
