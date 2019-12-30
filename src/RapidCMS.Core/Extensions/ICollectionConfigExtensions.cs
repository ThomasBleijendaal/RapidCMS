namespace RapidCMS.Core.Extensions
{
    public static class ICollectionConfigExtensions
    {
        // TODO
        //internal static List<Collection> ProcessCollections(this ICollectionConfig root)
        //{
        //    var list = new List<Collection>();

        //    foreach (var configReceiver in root.Collections.Cast<CollectionConfig>())
        //    {
        //        var collection = new Collection(
        //            configReceiver.Icon,
        //            configReceiver.Name,
        //            configReceiver.Alias,
        //            configReceiver.EntityVariant.ToEntityVariant(),
        //            configReceiver.RepositoryType,
        //            configReceiver.Recursive)
        //        {
        //            DataViews = configReceiver.DataViews,
        //            DataViewBuilder = configReceiver.DataViewBuilder
        //        };

        //        if (configReceiver.SubEntityVariants.Any())
        //        {
        //            collection.SubEntityVariants = configReceiver.SubEntityVariants.ToList(variant => variant.ToEntityVariant());
        //        }

        //        collection.TreeView = configReceiver.TreeView?.ToTreeView();

        //        collection.ListView = configReceiver.ListView?.ToList(collection);
        //        collection.ListEditor = configReceiver.ListEditor?.ToList(collection);

        //        collection.NodeView = configReceiver.NodeView?.ToNode(collection);
        //        collection.NodeEditor = configReceiver.NodeEditor?.ToNode(collection);

        //        collection.Collections = configReceiver.ProcessCollections();

        //        list.Add(collection);
        //    }

        //    return list;
        //}
    }
}
