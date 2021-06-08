using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Setup
{
    internal class ListSetup : IListSetup
    {
        public ListSetup(
            int? pageSize, 
            bool? searchBarVisible, 
            bool? reorderingAllowed, 
            ListType listType, 
            EmptyVariantColumnVisibility emptyVariantColumnVisibility, 
            List<IPaneSetup> panes, 
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

        public int? PageSize { get; set; }
        public bool? SearchBarVisible { get; set; }
        public bool? ReorderingAllowed { get; set; }
        public ListType ListType { get; set; }
        public EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        public List<IPaneSetup> Panes { get; set; }
        public List<IButtonSetup> Buttons { get; set; }

        public IButtonSetup? FindButton(string buttonId)
        {
            return GetAllButtons()?.FirstOrDefault(x => x.ButtonId == buttonId);
        }
        public IEnumerable<IButtonSetup>? GetAllButtons()
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
