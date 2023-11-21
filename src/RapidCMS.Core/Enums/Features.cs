using System;

namespace RapidCMS.Core.Enums;

[Flags]
public enum Features
{
    None = 0,
    CanGoToView = 1,
    CanGoToEdit = 2,
    CanView = 4,
    CanEdit = 8,
    IsBlockList = 65536
}
