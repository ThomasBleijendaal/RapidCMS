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
                CustomType = pane.CustomType,
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
}
