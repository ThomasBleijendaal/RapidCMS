using System;
using System.Linq;
using System.Reflection;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
    [DefaultType(typeof(string))]
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

        internal static Type GetDefaultValueMapper(Type valueType)
        {
            foreach (var type in Assembly.GetAssembly(typeof(ValueMapper<>)).GetTypes())
            {
                var attribute = type.GetCustomAttribute<DefaultTypeAttribute>();

                if (attribute?.Types.Contains(valueType) ?? false)
                {
                    return type;
                }
            }

            return typeof(DefaultValueMapper);
        }
    }
}
