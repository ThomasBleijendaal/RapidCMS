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

                // TODO: test if sub collections work in collections
                //{
                //    return new Pane
                //    {
                //        CustomAlias = pane.CustomAlias,
                //        IsVisible = pane.IsVisible,
                //        VariantType = pane.VariantType,
                //        Buttons = pane.Buttons.ToList(button => button.ToButton(collection.SubEntityVariants, collection.EntityVariant)),
                //        Fields = pane.Fields.ToList(field => field.ToField())
                //    };
                //})
            };
        }
    }
}
