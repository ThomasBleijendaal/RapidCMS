using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Setup
{
    public class NodeSetup
    {
        public NodeSetup(
            Type baseType, 
            List<PaneSetup> panes, 
            List<ButtonSetup> buttons)
        {
            BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
            Panes = panes ?? throw new ArgumentNullException(nameof(panes));
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
        }

        public Type BaseType { get; set; }
        public List<PaneSetup> Panes { get; set; }
        public List<ButtonSetup> Buttons { get; set; }

        public ButtonSetup? FindButton(string buttonId)
        {
            return GetAllButtons()?.FirstOrDefault(x => x.ButtonId == buttonId);
        }
        public IEnumerable<ButtonSetup>? GetAllButtons()
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
