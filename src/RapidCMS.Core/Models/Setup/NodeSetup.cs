using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Setup
{
    public sealed class NodeSetup
    {
        internal Type BaseType { get; set; }
        internal List<PaneSetup> Panes { get; set; }
        internal List<ButtonSetup> Buttons { get; set; }

        internal ButtonSetup? FindButton(string buttonId)
        {
            return GetAllButtons()?.FirstOrDefault(x => x.ButtonId == buttonId);
        }
        internal IEnumerable<ButtonSetup>? GetAllButtons()
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
