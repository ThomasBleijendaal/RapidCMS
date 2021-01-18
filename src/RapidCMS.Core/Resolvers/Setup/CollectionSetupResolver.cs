using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
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
        private readonly IRepositoryTypeResolver _repositoryTypeResolver;

        private Dictionary<string, CollectionConfig> _collectionMap { get; set; } = new Dictionary<string, CollectionConfig>();
        private Dictionary<string, CollectionSetup> _cachedCollectionMap { get; set; } = new Dictionary<string, CollectionSetup>();

        public CollectionSetupResolver(ICmsConfig cmsConfig,
            ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> treeElementResolver,
            ISetupResolver<IEntityVariantSetup, EntityVariantConfig> entityVariantResolver,
            ISetupResolver<TreeViewSetup, TreeViewConfig> treeViewResolver,
            ISetupResolver<ListSetup, ListConfig> listResolver,
            ISetupResolver<NodeSetup, NodeConfig> nodeResolver,
            IRepositoryTypeResolver repositoryTypeResolver)
        {
            _treeElementResolver = treeElementResolver;
            _entityVariantResolver = entityVariantResolver;
            _treeViewResolver = treeViewResolver;
            _listResolver = listResolver;
            _nodeResolver = nodeResolver;
            _repositoryTypeResolver = repositoryTypeResolver;
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
            throw new InvalidOperationException("Cannot resolve collection or page without alias.");
        }

        ICollectionSetup ISetupResolver<ICollectionSetup>.ResolveSetup(string alias)
        {
            if (_cachedCollectionMap.TryGetValue(alias, out var collectionSetup))
            {
                return collectionSetup;
            }
            // TODO: this can be buggy when multiple collection use the same repo
            else if (_cachedCollectionMap.FirstOrDefault(x => x.Value.RepositoryAlias == alias).Value is CollectionSetup collection)
            {
                return collection;
            }

            if (_collectionMap.TryGetValue(alias, out var collectionConfig))
            {
                var resolvedSetup = ConvertConfig(collectionConfig);
                if (resolvedSetup.Cachable)
                {
                    _cachedCollectionMap[alias] = resolvedSetup.Setup;
                }

                return resolvedSetup.Setup;
            }
            else
            {
                throw new InvalidOperationException($"Cannot find collection with alias {alias}.");
            }
        }

        private IResolvedSetup<CollectionSetup> ConvertConfig(CollectionConfig config)
        {
            var repositoryAlias = _repositoryTypeResolver.GetAlias(config.RepositoryType);

            var collection = new CollectionSetup(
                config.Icon,
                config.Color,
                config.Name,
                config.Alias,
                repositoryAlias,
                isRecursive: config.Recursive,
                isResolverCachable: true) // TODO
            {
                DataViews = config.DataViews,
                DataViewBuilder = config.DataViewBuilder
            };

            var cacheable = true;

            if (!string.IsNullOrWhiteSpace(config.ParentAlias) && _collectionMap.TryGetValue(config.ParentAlias, out var collectionConfig))
            {
                collection.Parent = new TreeElementSetup(collectionConfig.Alias, PageType.Collection); // TODO: enum
            }
            collection.Collections = _treeElementResolver.ResolveSetup(config.CollectionsAndPages, collection).CheckIfCachable(ref cacheable).ToList();

            collection.EntityVariant = _entityVariantResolver.ResolveSetup(config.EntityVariant, collection).CheckIfCachable(ref cacheable);
            if (config.SubEntityVariants.Any())
            {
                collection.SubEntityVariants = _entityVariantResolver.ResolveSetup(config.SubEntityVariants, collection).CheckIfCachable(ref cacheable).ToList();
            }

            collection.TreeView = config.TreeView == null ? null : _treeViewResolver.ResolveSetup(config.TreeView, collection).CheckIfCachable(ref cacheable);

            collection.ListView = config.ListView == null ? null : _listResolver.ResolveSetup(config.ListView, collection).CheckIfCachable(ref cacheable);
            collection.ListEditor = config.ListEditor == null ? null : _listResolver.ResolveSetup(config.ListEditor, collection).CheckIfCachable(ref cacheable);

            collection.NodeView = config.NodeView == null ? null : _nodeResolver.ResolveSetup(config.NodeView, collection).CheckIfCachable(ref cacheable);
            collection.NodeEditor = config.NodeEditor == null ? null : _nodeResolver.ResolveSetup(config.NodeEditor, collection).CheckIfCachable(ref cacheable);

            return new ResolvedSetup<CollectionSetup>(collection, cacheable);
        }
    }
}
