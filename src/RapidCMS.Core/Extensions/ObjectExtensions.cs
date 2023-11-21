using System;

namespace RapidCMS.Core.Extensions;

public static class ObjectExtensions
{
    public static T If<T>(this T subject, bool shouldDo, Func<T, T> method)
    {
        return shouldDo
            ? method.Invoke(subject)
            : subject;
    }
}
