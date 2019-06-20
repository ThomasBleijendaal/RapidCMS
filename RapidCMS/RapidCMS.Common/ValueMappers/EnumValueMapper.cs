using System;

namespace RapidCMS.Common.ValueMappers
{
    public class EnumValueMapper<TEnum> : ValueMapper<TEnum>
        where TEnum : struct, Enum
    {
        public override TEnum MapFromEditor(object value)
        {
            return Enum.TryParse<TEnum>(value.ToString(), out var enumValue) ? enumValue : default;
        }

        public override object MapToEditor(TEnum value)
        {
            return Convert.ToInt32(value);
        }
    }
}
