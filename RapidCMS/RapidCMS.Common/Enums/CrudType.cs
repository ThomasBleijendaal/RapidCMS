using System;

namespace RapidCMS.Common.Enums
{
    [Flags]
    public enum CrudType
    {
        Create = 1,
        Read = 2,
        Insert = 4,
        Update = 8,
        Delete = 16
    }
}
