using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    public class BoolValueMapper : ValueMapper<bool?>
    {
        public override bool? MapFromEditor(ValueMappingContext context, object value)
        {
            return value == null ? default(bool?) : (bool)value;
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
