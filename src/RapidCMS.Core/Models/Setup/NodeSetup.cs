using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class NodeSetup
    {
        internal NodeSetup(NodeConfig node, CollectionSetup collection)
        {
            BaseType = node.BaseType;
            Buttons = node.Buttons.ToList(button => new ButtonSetup(button, collection.EntityVariant, collection.SubEntityVariants));
            Panes = node.Panes.ToList(pane => new PaneSetup(pane));
        }

        internal Type BaseType { get; set; }
        internal List<PaneSetup> Panes { get; set; }
        internal List<ButtonSetup> Buttons { get; set; }

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
