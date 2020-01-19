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
        Reordered = 32,

        [Obsolete("Replace with PageType")]
        List = 65536,

        [Obsolete("Replace with PageType")]
        Node = 131072,

        Root = 2097152,
        NotRoot = 4194304
    }
}
