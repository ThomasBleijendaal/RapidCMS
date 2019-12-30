using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config
{
    internal class PaneButtonConfig : ButtonConfig
    {
        internal PaneButtonConfig(Type paneType, CrudType? crudType)
        {
            PaneType = paneType ?? throw new ArgumentNullException(nameof(paneType));
            CrudType = crudType;
        }

        internal Type PaneType { get; set; }
        internal CrudType? CrudType { get; set; }
    }
}
