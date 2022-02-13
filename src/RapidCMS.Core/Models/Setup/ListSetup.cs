using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Setup
{
    public class ListSetup
    {
        public ListSetup(
            int? pageSize, 
            bool? searchBarVisible, 
            bool? reorderingAllowed, 
            ListType listType, 
            EmptyVariantColumnVisibility emptyVariantColumnVisibility, 
            List<PaneSetup> panes, 
            List<ButtonSetup> buttons)
        {
            PageSize = pageSize;
            SearchBarVisible = searchBarVisible;
            ReorderingAllowed = reorderingAllowed;
            ListType = listType;
            EmptyVariantColumnVisibility = emptyVariantColumnVisibility;
            Panes = panes ?? throw new ArgumentNullException(nameof(panes));
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
        }

        public int? PageSize { get; set; }
        public bool? SearchBarVisible { get; set; }
        public bool? ReorderingAllowed { get; set; }
        public ListType ListType { get; set; }
        public EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
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
