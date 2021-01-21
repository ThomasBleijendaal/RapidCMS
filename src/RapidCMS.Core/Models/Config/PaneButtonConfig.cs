using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config
{
    internal class PaneButtonConfig : ButtonConfig
    {
        internal PaneButtonConfig(Type paneType)
        {
            PaneType = paneType ?? throw new ArgumentNullException(nameof(paneType));
        }

        internal Type PaneType { get; set; }
    }
}
