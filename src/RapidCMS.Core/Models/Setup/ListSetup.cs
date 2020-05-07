using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Setup
{
    internal class ListSetup
    {
        public ListSetup(
            int? pageSize, 
            bool? searchBarVisible, 
            bool? reorderingAllowed, 
            ListType listType, 
            EmptyVariantColumnVisibility emptyVariantColumnVisibility, 
            List<PaneSetup> panes, 
            List<IButtonSetup> buttons)
        {
            PageSize = pageSize;
            SearchBarVisible = searchBarVisible;
            ReorderingAllowed = reorderingAllowed;
            ListType = listType;
            EmptyVariantColumnVisibility = emptyVariantColumnVisibility;
            Panes = panes ?? throw new ArgumentNullException(nameof(panes));
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
        }

        internal int? PageSize { get; set; }
        internal bool? SearchBarVisible { get; set; }
        internal bool? ReorderingAllowed { get; set; }
        internal ListType ListType { get; set; }
        internal EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
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
