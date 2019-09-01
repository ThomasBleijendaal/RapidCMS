using System;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public class PaneButtonConfig : ButtonConfig
    {
        public PaneButtonConfig(Type paneType, CrudType? crudType)
        {
            PaneType = paneType ?? throw new ArgumentNullException(nameof(paneType));
            CrudType = crudType ?? throw new ArgumentNullException(nameof(crudType));
        }

        internal Type PaneType { get; set; }
        internal CrudType CrudType { get; set; }
    }
}
