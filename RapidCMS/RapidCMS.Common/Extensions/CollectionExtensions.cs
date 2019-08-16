using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    public static class ICollectionRootExtensions
    {
        public static ICollectionRoot AddCollection<TEntity>(this ICollectionRoot root, string alias, string name, Action<CollectionConfig<TEntity>> configure)
            where TEntity : IEntity
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            if (alias != alias.ToUrlFriendlyString())
            {
                throw new ArgumentException($"Use lowercase, hyphened strings as alias for collections, '{alias.ToUrlFriendlyString()}' instead of '{alias}'.");
            }
            if (!root.IsUnique(alias))
            {
                throw new NotUniqueException(nameof(alias));
            }

            var configReceiver = new CollectionConfig<TEntity>(alias, name,
                new EntityVariantConfig(typeof(TEntity).Name, typeof(TEntity)));

            configure.Invoke(configReceiver);

            root.Collections.Add(configReceiver);

            return root;
        }

        public static ICollectionRoot AddSelfAsRecursiveCollection<TEntity>(this CollectionConfig<TEntity> root)
            where TEntity : IEntity
        {
            var configReceiver = new CollectionConfig<TEntity>(root.Alias, root.Name, root.EntityVariant)
            {
                Recursive = true
            };

            configReceiver.RepositoryType = root.RepositoryType ?? throw new InvalidOperationException("Cannot add self without a Repository, use SetRepository first.");

            root.Collections.Add(configReceiver);

            return root;
        }

        public static List<Collection> ProcessCollections(this ICollectionRoot root)
        {
            var list = new List<Collection>();

            foreach (var configReceiver in root.Collections)
            {
                var collection = new Collection(
                    configReceiver.Name,
                    configReceiver.Alias,
                    configReceiver.EntityVariant.ToEntityVariant(),
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

                collection.TreeView = configReceiver.TreeView?.ToTreeView();

                collection.ListView = configReceiver.ListView?.ToList(collection);
                collection.ListEditor = configReceiver.ListEditor?.ToList(collection);

                collection.NodeView = configReceiver.NodeView?.ToNode(collection);
                collection.NodeEditor = configReceiver.NodeEditor?.ToNode(collection);

                collection.Collections = configReceiver.ProcessCollections();

                list.Add(collection);
            }

            return list;
        }
    }
}
