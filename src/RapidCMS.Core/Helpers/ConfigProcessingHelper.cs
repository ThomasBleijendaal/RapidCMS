using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Interfaces.Config;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Helpers
{
    internal static class ConfigProcessingHelper
    {
        internal static List<CollectionSetup> ProcessCollections(this ICollectionConfig root)
        {
            var list = new List<CollectionSetup>();

            foreach (var configReceiver in root.Collections.Cast<CollectionConfig>())
            {
                var collection = new CollectionSetup(
                    configReceiver.Icon,
                    configReceiver.Name,
                    configReceiver.Alias,
                    new EntityVariantSetup(configReceiver.EntityVariant),
                    configReceiver.RepositoryType,
                    configReceiver.Recursive)
                {
                    DataViews = configReceiver.DataViews,
                    DataViewBuilder = configReceiver.DataViewBuilder
                };

                if (configReceiver.SubEntityVariants.Any())
                {
                    collection.SubEntityVariants = configReceiver.SubEntityVariants.ToList(variant => variant.ToEntityVariant());
                }

                collection.TreeView = configReceiver.TreeView == null ? null : new TreeViewSetup(configReceiver.TreeView);

                collection.ListView = configReceiver.ListView?.ToList(collection);
                collection.ListEditor = configReceiver.ListEditor?.ToList(collection);

                collection.NodeView = configReceiver.NodeView?.ToNode(collection);
                collection.NodeEditor = configReceiver.NodeEditor?.ToNode(collection);

                collection.Collections = configReceiver.ProcessCollections();

                list.Add(collection);
            }

            return list;
        }

        public static ListSetup ToList(this ListConfig list, Collection collection)
        {
            return new ListSetup
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
