using System;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    public class EnumValueMapper<TEnum> : ValueMapper<TEnum>
        where TEnum : struct, Enum
    {
        public override TEnum MapFromEditor(ValueMappingContext context, object value)
        {
            return Enum.TryParse<TEnum>(value.ToString(), out var enumValue) ? enumValue : default;
        }

        public override object MapToEditor(ValueMappingContext context, TEnum value)
        {
            return Convert.ToInt32(value);
        }

        public override string MapToView(ValueMappingContext context, TEnum value)
        {
            return value.ToString();
        }
    }
}
