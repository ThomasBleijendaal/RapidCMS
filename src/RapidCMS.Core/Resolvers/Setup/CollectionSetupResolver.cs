using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Config.Convention;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class CollectionSetupResolver : ISetupResolver<ICollectionSetup>
    {
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> _treeElementResolver;
        private readonly ISetupResolver<IEntityVariantSetup, EntityVariantConfig> _entityVariantResolver;
        private readonly ISetupResolver<TreeViewSetup, TreeViewConfig> _treeViewResolver;
        private readonly ISetupResolver<ListSetup, ListConfig> _listResolver;
        private readonly ISetupResolver<NodeSetup, NodeConfig> _nodeResolver;

        private Dictionary<string, CollectionConfig> _collectionMap { get; set; } = new Dictionary<string, CollectionConfig>();
        private Dictionary<string, CollectionSetup> _cachedCollectionMap { get; set; } = new Dictionary<string, CollectionSetup>();

        public CollectionSetupResolver(ICmsConfig cmsConfig,
            ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> treeElementResolver,
            ISetupResolver<IEntityVariantSetup, EntityVariantConfig> entityVariantResolver,
            ISetupResolver<TreeViewSetup, TreeViewConfig> treeViewResolver,
            ISetupResolver<ListSetup, ListConfig> listResolver,
            ISetupResolver<NodeSetup, NodeConfig> nodeResolver)
        {
            _treeElementResolver = treeElementResolver;
            _entityVariantResolver = entityVariantResolver;
            _treeViewResolver = treeViewResolver;
            _listResolver = listResolver;
            _nodeResolver = nodeResolver;
            Initialize(cmsConfig);
        }

        private void Initialize(ICmsConfig cmsConfig)
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

                // TODO: set this property
                if (collectionSetup.ResolverCachable)
                {
                    _cachedCollectionMap[alias] = collectionSetup;
                }

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
                config.RepositoryType,
                isRecursive: config.Recursive,
                isResolverCachable: true) // TODO
            {
                DataViews = config.DataViews,
                DataViewBuilder = config.DataViewBuilder
            };

            collection.EntityVariant = _entityVariantResolver.ResolveSetup(config.EntityVariant, collection);
            if (config.SubEntityVariants.Any())
            {
                collection.SubEntityVariants = _entityVariantResolver.ResolveSetup(config.SubEntityVariants, collection).ToList();
            }

            collection.TreeView =config.TreeView == null ? null :  _treeViewResolver.ResolveSetup(config.TreeView, collection);

            collection.ListView = config.ListView == null ? null : _listResolver.ResolveSetup(config.ListView, collection);
            collection.ListEditor = config.ListEditor == null ? null : _listResolver.ResolveSetup(config.ListEditor, collection);

            collection.NodeView = config.NodeView == null ? null : _nodeResolver.ResolveSetup(config.NodeView, collection);
            collection.NodeEditor = config.NodeEditor == null ? null : _nodeResolver.ResolveSetup(config.NodeEditor, collection);

            collection.Collections = _treeElementResolver.ResolveSetup(config.CollectionsAndPages, collection).ToList();

            return collection;
        }
    }
}
