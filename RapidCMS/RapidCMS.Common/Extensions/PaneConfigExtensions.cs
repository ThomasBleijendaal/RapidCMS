using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class PaneConfigExtensions
    {
        public static Pane ToPane(this PaneConfig pane)
        {
            return new Pane
            {
                CustomAlias = pane.CustomAlias,
                IsVisible = pane.IsVisible,
                Label = pane.Label,
                VariantType = pane.VariantType,
                Buttons = pane.Buttons.ToList(button => button.ToButton()),
                Fields = pane.Fields.ToList(x => x.ToField()),
                SubCollectionLists = pane.SubCollectionLists.ToList(x => x.ToSubCollectionList()),
                RelatedCollectionLists = pane.RelatedCollectionLists.ToList(x => x.ToRelatedCollectionList())
            };
        }
    }

    internal static class NodeConfigExtensions
    {
        public static Node ToNode(this NodeConfig node, Collection collection)
        {
            return new Node
            {
                Buttons = node.Buttons.ToList(button => button.ToButton(collection.SubEntityVariants, collection.EntityVariant)),
                BaseType = node.BaseType,
                EditorPanes = node.Panes.ToList(config => config.ToPane())
            };
        }
    }

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
                Panes = list.Panes.ToList(pane =>
                {
                    return new Pane
                    {
                        CustomAlias = pane.CustomAlias,
                        IsVisible = pane.IsVisible,
                        VariantType = pane.VariantType,
                        Buttons = pane.Buttons.ToList(button => button.ToButton(collection.SubEntityVariants, collection.EntityVariant)),
                        Fields = pane.Fields.ToList(field => field.ToField())
                    };
                })
            };
        }
    }
}
