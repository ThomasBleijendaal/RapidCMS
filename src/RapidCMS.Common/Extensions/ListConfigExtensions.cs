using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class ListConfigExtensions
    {
        public static List ToList(this ListConfig list, Collection collection)
        {
            return new List
            {
                PageSize = list.PageSize,
                SearchBarVisible = list.SearchBarVisible,
                ListType = list.ListEditorType,
                EmptyVariantColumnVisibility = list.EmptyVariantColumnVisibility,
                Buttons = list.Buttons.ToList(button => button.ToButton(collection.SubEntityVariants, collection.EntityVariant)),
                Panes = list.Panes.ToList(pane => pane.ToPane())
            };
        }
    }
}
