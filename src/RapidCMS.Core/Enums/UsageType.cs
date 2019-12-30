using System;

namespace RapidCMS.Core.Enums
{
    [Flags]
    public enum UsageType
    {
        View = 1,
        New = 2,
        Edit = 4,
        Add = 8,
        Pick = 16,

        List = 65536,
        Node = 131072,

        Root = 2097152,
        NotRoot = 4194304
    }
}
