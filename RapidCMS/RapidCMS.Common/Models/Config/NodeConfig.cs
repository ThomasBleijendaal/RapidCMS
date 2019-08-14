using System;
using System.Collections.Generic;

namespace RapidCMS.Common.Models.Config
{
    public class NodeConfig
    {
        public NodeConfig(Type baseType)
        {
            BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
        }

        internal Type BaseType { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<PaneConfig> Panes { get; set; } = new List<PaneConfig>();
    }
}
