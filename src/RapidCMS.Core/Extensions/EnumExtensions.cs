using System;
using System.Linq;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Extensions;

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

            return attribute as TAttribute;
        }

        return default;
    }

    public static UsageType FindSupportedUsageType(this UsageType supportsUsageType, UsageType requestedUsageType)
    {
        var supportedUsageType = (supportsUsageType & requestedUsageType) & UsageType.ViewOrEdit;

        if (supportedUsageType > 0)
        {
            return supportedUsageType;
        }
        else
        {
            return UsageType.View | (requestedUsageType & ~UsageType.ViewOrEdit);
        } 
    }
}
