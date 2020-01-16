using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class ListSetup
    {
        internal ListSetup(ListConfig list, CollectionSetup collection)
        {
            PageSize = list.PageSize;
            SearchBarVisible = list.SearchBarVisible;
            ReorderingAllowed = list.ReorderingAllowed;
            ListType = list.ListEditorType;
            EmptyVariantColumnVisibility = list.EmptyVariantColumnVisibility;
            Buttons = list.Buttons.ToList(button => new ButtonSetup(button, collection.EntityVariant, collection.SubEntityVariants));
            Panes = list.Panes.ToList(pane => new PaneSetup(pane));
        }

        internal int? PageSize { get; set; }
        internal bool? SearchBarVisible { get; set; }
        internal bool? ReorderingAllowed { get; set; }
        internal ListType ListType { get; set; }
        internal EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        internal List<PaneSetup>? Panes { get; set; }
        internal List<ButtonSetup>? Buttons { get; set; }

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
