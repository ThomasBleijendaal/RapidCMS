using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class CollectionSetupResolver : ISetupResolver<ICollectionSetup>
    {
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> _treeElementResolver;

        private Dictionary<string, CollectionConfig> _collectionMap { get; set; } = new Dictionary<string, CollectionConfig>();
        private Dictionary<string, CollectionSetup> _cachedCollectionMap { get; set; } = new Dictionary<string, CollectionSetup>();

        public CollectionSetupResolver(ICmsConfig cmsConfig,
            ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> treeElementResolver)
        {
            MapCollections(cmsConfig.CollectionsAndPages.SelectNotNull(x => x as CollectionConfig));

            void MapCollections(IEnumerable<CollectionConfig> collections)
            {
                foreach (var collection in collections.Where(col => !col.Recursive))
                {
                    if (!_collectionMap.TryAdd(collection.Alias, collection))
                    {
                        throw new InvalidOperationException($"Duplicate collection alias '{collection.Alias}' not allowed.");
                    }

                    var subCollections = collection.CollectionsAndPages.SelectNotNull(x => x as CollectionConfig);
                    if (subCollections.Any())
                    {
                        MapCollections(subCollections);
                    }
                }
            }
            _treeElementResolver = treeElementResolver;
        }

        ICollectionSetup ISetupResolver<ICollectionSetup>.ResolveSetup()
        {
            throw new InvalidOperationException("Cannot collection page without alias.");
        }

        ICollectionSetup ISetupResolver<ICollectionSetup>.ResolveSetup(string alias)
        {
            if (_cachedCollectionMap.TryGetValue(alias, out var collectionSetup))
            {
                return collectionSetup;
            }

            if (_collectionMap.TryGetValue(alias, out var collectionConfig))
            {
                collectionSetup = ConvertConfig(collectionConfig);

                // TODO: test if the collection allows this
                _cachedCollectionMap[alias] = collectionSetup;

                return collectionSetup;
            }
            else
            {
                throw new InvalidOperationException($"Cannot find collection with alias {alias}.");
            }
        }

        private CollectionSetup ConvertConfig(CollectionConfig config)
        {
            var collection = new CollectionSetup(
                config.Icon,
                config.Name,
                config.Alias,
                new EntityVariantSetup(config.EntityVariant),
                config.RepositoryType,
                config.Recursive)
            {
                DataViews = config.DataViews,
                DataViewBuilder = config.DataViewBuilder
            };

            if (config.SubEntityVariants.Any())
            {
                collection.SubEntityVariants = config.SubEntityVariants.ToList(variant => new EntityVariantSetup(variant));
            }

            collection.TreeView = config.TreeView == null ? null : new TreeViewSetup(config.TreeView);

            collection.ListView = config.ListView == null ? null : new ListSetup(config.ListView, collection);
            collection.ListEditor = config.ListEditor == null ? null : new ListSetup(config.ListEditor, collection);

            collection.NodeView = config.NodeView == null ? null : new NodeSetup(config.NodeView, collection);
            collection.NodeEditor = config.NodeEditor == null ? null : new NodeSetup(config.NodeEditor, collection);

            collection.Collections = _treeElementResolver.ResolveSetup(config.CollectionsAndPages).ToList();

            return collection;
        }
    }
}
