using System;
using System.Linq;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Extensions
{
    internal static class EnumExtensions
    {
        public static TAttribute? GetCustomAttribute<TAttribute>(this Enum e)
            where TAttribute : Attribute
        {
            var member = e.GetType().GetMember(e.ToString()).FirstOrDefault();

            if (member != null)
            {
                var attribute = member
                    .GetCustomAttributes(typeof(TAttribute), true)
                    ?.FirstOrDefault();
                ;

                return attribute as TAttribute;
            }

            return default;
        }

        public static UsageType FindSupportedUsageType(this UsageType supportsUsageType, UsageType requestedUsageType)
        {
            if (requestedUsageType.HasFlag(supportsUsageType))
            {
                return requestedUsageType;
            }
            else
            {
                // The SupportUsageType is only Edit or View, so remove those from requested type and add the supported
                // so it won't mess with Node / List UsageTypes
                return (requestedUsageType & ~(UsageType.Edit | UsageType.View)) | supportsUsageType;
            }
        }
    }
}
