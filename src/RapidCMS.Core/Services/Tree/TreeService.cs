using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.State;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Services.Tree
{
    internal class TreeService : ITreeService
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly ISetupResolver<IPageSetup> _pageResolver;
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>> _treeElementResolver;
        private readonly ICms _cms;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IAuthService _authService;
        private readonly IParentService _parentService;

        public TreeService(
            ISetupResolver<ICollectionSetup> collectionResolver,
            ISetupResolver<IPageSetup> pageResolver,
            ISetupResolver<IEnumerable<ITreeElementSetup>> treeElementResolver,
            ICms cms,
            IRepositoryResolver repositoryResolver,
            IAuthService authService,
            IParentService parentService)
        {
            _collectionResolver = collectionResolver;
            _pageResolver = pageResolver;
            _treeElementResolver = treeElementResolver;
            _cms = cms;
            _repositoryResolver = repositoryResolver;
            _authService = authService;
            _parentService = parentService;
        }

        public async Task<TreeCollectionUI?> GetCollectionAsync(string alias, ParentPath? parentPath)
        {
            try
            {
                var collection = _collectionResolver.ResolveSetup(alias);
                if (collection == null)
                {
                    throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
                }

                var parent = await _parentService.GetParentAsync(parentPath).ConfigureAwait(false);
                var repositoryContext = new RepositoryContext(collection.Alias);

                var testEntity = await _repositoryResolver.GetRepository(collection).NewAsync(repositoryContext, parent, collection.EntityVariant.Type).ConfigureAwait(false);

                var tree = new TreeCollectionUI(collection.Alias, collection.Name)
                {
                    EntitiesVisible = collection.TreeView?.EntityVisibility == EntityVisibilty.Visible,
                    RootVisible = collection.TreeView?.RootVisibility == CollectionRootVisibility.Visible,
                    Icon = collection.Icon ?? "list",
                    DefaultOpenEntities = collection.TreeView?.DefaultOpenEntities ?? false
                };

                if (collection.ListEditor != null && await _authService.IsUserAuthorizedAsync(Operations.Update, testEntity).ConfigureAwait(false))
                {
                    tree.State = new PageStateModel
                    {
                        CollectionAlias = collection.Alias,
                        PageType = PageType.Collection,
                        ParentPath = parentPath,
                        UsageType = UsageType.Edit
                    };
                }
                else if (collection.ListView != null && await _authService.IsUserAuthorizedAsync(Operations.Read, testEntity).ConfigureAwait(false))
                {
                    tree.State = new PageStateModel
                    {
                        CollectionAlias = collection.Alias,
                        PageType = PageType.Collection,
                        ParentPath = parentPath,
                        UsageType = UsageType.View
                    };
                }

                return tree;
            }
            catch (Exception ex) 
            {
                Console.Write(ex);
                return null;
            }
        }

        public Task<TreePageUI?> GetPageAsync(string alias)
        {
            var page = _pageResolver.ResolveSetup(alias);
            if (page == null)
            {
                throw new InvalidOperationException($"Failed to get page for given alias ({alias}).");
            }

            return Task.FromResult(new TreePageUI(page.Name, page.Icon, new PageStateModel
            {
                CollectionAlias = page.Alias,
                PageType = PageType.Page
            }))!;
        }

        public async Task<TreeNodesUI?> GetNodesAsync(string alias, ParentPath? parentPath, int pageNr, int pageSize)
        {
            var collection = _collectionResolver.ResolveSetup(alias);
            if (collection == null)
            {
                throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
            }

            var parent = await _parentService.GetParentAsync(parentPath).ConfigureAwait(false);

            if (collection.TreeView?.EntityVisibility == EntityVisibilty.Visible)
            {
                var query = Query.Create(pageSize, pageNr, default, default);
                var repositoryContext = new RepositoryContext(collection.Alias);
                var entities = await _repositoryResolver.GetRepository(collection).GetAllAsync(repositoryContext, parent, query).ConfigureAwait(false);

                var list = await entities.ToListAsync(async entity =>
                {
                    var entityVariant = collection.GetEntityVariant(entity);

                    var node = new TreeNodeUI(
                        entity.Id!,
                        collection.TreeView.Name!.StringGetter.Invoke(entity),
                        collection.Collections.ToList(subCollection => (subCollection.Alias, PageType.Collection)))
                    {
                        RootVisibleOfCollections = collection.Collections.All(subCollection => subCollection.RootVisibility == CollectionRootVisibility.Visible),
                        DefaultOpenCollections = collection.TreeView?.DefaultOpenCollections ?? false
                    };

                    if (collection.NodeEditor != null && await _authService.IsUserAuthorizedAsync(Operations.Update, entity).ConfigureAwait(false))
                    {
                        node.State = new PageStateModel
                        {
                            CollectionAlias = collection.Alias,
                            Id = entity.Id,
                            PageType = PageType.Node,
                            ParentPath = parentPath,
                            UsageType = UsageType.Edit,
                            VariantAlias = entityVariant.Alias
                        };
                    }
                    else if (collection.ListView != null && await _authService.IsUserAuthorizedAsync(Operations.Read, entity).ConfigureAwait(false))
                    {
                        node.State = new PageStateModel
                        {
                            CollectionAlias = collection.Alias,
                            Id = entity.Id,
                            PageType = PageType.Node,
                            ParentPath = parentPath,
                            UsageType = UsageType.View,
                            VariantAlias = entityVariant.Alias
                        };
                    }

                    return node;
                }).ConfigureAwait(false);

                return new TreeNodesUI(list.Take(pageSize).ToList(), query.MoreDataAvailable);
            }
            else
            {
                return default;
            }
        }

        public IDisposable SubscribeToRepositoryUpdates(string alias, Func<Task> asyncCallback)
        {
            return _repositoryResolver.GetRepository(alias).ChangeToken.RegisterChangeCallback((x) => asyncCallback.Invoke(), null);
        }

        public TreeRootUI GetRoot()
        {
            var collections = _treeElementResolver.ResolveSetup()
                .ToList(x => (x.Alias, x.Type));

            return new TreeRootUI("-1", _cms.SiteName, collections)
            {
                State = new PageStateModel
                {
                    PageType = PageType.Dashboard
                }
            };
        }
    }
}
