using RapidCMS.Common.Attributes;
using System;

namespace RapidCMS.Common.Enums
{
    [Flags]
    public enum UsageType
    {
        View = 1,
        New = 2,
        Edit = 4,

        List = 65536,
        Node = 131072
    }
}
