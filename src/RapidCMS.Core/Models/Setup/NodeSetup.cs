using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Setup
{
    internal class NodeSetup
    {
        public NodeSetup(
            Type baseType, 
            List<PaneSetup> panes, 
            List<IButtonSetup> buttons)
        {
            BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
            Panes = panes ?? throw new ArgumentNullException(nameof(panes));
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
        }

        internal Type BaseType { get; set; }
        internal List<PaneSetup> Panes { get; set; }
        internal List<IButtonSetup> Buttons { get; set; }

        internal IButtonSetup? FindButton(string buttonId)
        {
            return GetAllButtons()?.FirstOrDefault(x => x.ButtonId == buttonId);
        }
        internal IEnumerable<IButtonSetup>? GetAllButtons()
        {
            if (Buttons != null)
            {
                foreach (var button in Buttons.GetAllButtons())
                {
                    yield return button;
                }
            }
            if (Panes != null)
            {
                foreach (var button in Panes.SelectMany(pane => pane.Buttons.GetAllButtons()))
                {
                    yield return button;
                }
            }
        }
    }
}
