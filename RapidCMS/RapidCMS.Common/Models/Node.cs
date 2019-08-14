using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Models
{
    internal class Node
    {
        internal Type BaseType { get; set; }
        internal List<Pane> EditorPanes { get; set; }
        internal List<Button> Buttons { get; set; }

        public Button? FindButton(string buttonId)
        {
            return GetAllButtons().FirstOrDefault(x => x.ButtonId == buttonId);
        }
        private IEnumerable<Button>? GetAllButtons()
        {
            if (Buttons != null)
            {
                foreach (var button in Buttons.GetAllButtons())
                {
                    yield return button;
                }
            }
            if (EditorPanes != null)
            {
                foreach (var button in EditorPanes.SelectMany(pane => pane.Buttons.GetAllButtons()))
                {
                    yield return button;
                }
            }
        }
    }
}
