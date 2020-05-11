using System;

namespace RapidCMS.Core.Enums
{
    [Flags]
    public enum Features
    {
        None = 0,
        CanGoToEdit = 1,
        CanEdit = 2
    }
}
