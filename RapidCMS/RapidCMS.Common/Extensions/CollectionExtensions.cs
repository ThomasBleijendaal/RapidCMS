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

            var configReceiver = new CollectionConfig<TEntity>
            {
                Alias = alias ?? throw new ArgumentNullException(nameof(alias)),
                Name = name ?? throw new ArgumentNullException(nameof(name)),
                EntityVariant = new EntityVariantConfig(typeof(TEntity).Name, typeof(TEntity))
            };

            configure.Invoke(configReceiver);

            root.Collections.Add(configReceiver);

            return root;
        }

        public static ICollectionRoot AddSelfAsRecursiveCollection<TEntity>(this CollectionConfig<TEntity> root)
            where TEntity : IEntity
        {
            var configReceiver = new CollectionConfig<TEntity>
            {
                Alias = root.Alias,
                Name = root.Name,
                EntityVariant = root.EntityVariant,

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
                // TODO: todo
                var variant = new EntityVariant
                {
                    Alias = configReceiver.EntityVariant.Type.FullName.ToUrlFriendlyString(),
                    Icon = null,
                    Name = configReceiver.EntityVariant.Name,
                    Type = configReceiver.EntityVariant.Type
                };

                var collection = new Collection(configReceiver.Name, configReceiver.Alias, variant, configReceiver.RepositoryType, configReceiver.Recursive)
                {
                    DataViews = configReceiver.DataViews,
                    DataViewBuilder = configReceiver.DataViewBuilder
                };

                if (configReceiver.TreeView != null)
                {
                    collection.TreeView = new TreeView
                    {
                        EntityVisibility = configReceiver.TreeView.EntityVisibilty,
                        RootVisibility = configReceiver.TreeView.RootVisibility,
                        Name = configReceiver.TreeView.Name
                    };
                }

                if (configReceiver.SubEntityVariants.Any())
                {
                    collection.SubEntityVariants = configReceiver.SubEntityVariants.ToList(variant => new EntityVariant
                    {
                        Alias = variant.Type.FullName.ToUrlFriendlyString(),
                        Icon = variant.Icon,
                        Name = variant.Name,
                        Type = variant.Type
                    });
                }

                // done
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
