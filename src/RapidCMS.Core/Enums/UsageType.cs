using System;

namespace RapidCMS.Core.Enums
{
    [Flags]
    public enum UsageType
    {
        None = 0,

        View = 1,
        New = 2,
        Edit = 4,
        Add = 8,
        Pick = 16,
        Reordered = 32,

        List = 65536,
        Node = 131072,
        Details = 262144,

        // TODO: why these elements? -- document why
        Root = 2097152,
        NotRoot = 4194304
    }
}
