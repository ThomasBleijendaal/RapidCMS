using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.State;
using RapidCMS.Core.Models.UI;

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
        private readonly IConcurrencyService _concurrencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TreeService(
            ISetupResolver<ICollectionSetup> collectionResolver,
            ISetupResolver<IPageSetup> pageResolver,
            ISetupResolver<IEnumerable<ITreeElementSetup>> treeElementResolver,
            ICms cms,
            IRepositoryResolver repositoryResolver,
            IAuthService authService,
            IParentService parentService,
            IConcurrencyService concurrencyService,
            IHttpContextAccessor httpContextAccessor)
        {
            _collectionResolver = collectionResolver;
            _pageResolver = pageResolver;
            _treeElementResolver = treeElementResolver;
            _cms = cms;
            _repositoryResolver = repositoryResolver;
            _authService = authService;
            _parentService = parentService;
            _concurrencyService = concurrencyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TreeCollectionUI?> GetCollectionAsync(string alias, ParentPath? parentPath)
        {
            var collection = await _collectionResolver.ResolveSetupAsync(alias);
            if (collection == null)
            {
                throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            var isList = collection.UsageType.HasFlag(UsageType.List);
            var isListEditor = collection.ListEditor != null;
            var isListView = collection.ListView != null;
            var isDetails = collection.UsageType.HasFlag(UsageType.Details) && parent?.Entity != null;

            var respository = _repositoryResolver.GetRepository(collection);
            var entity = await _concurrencyService.EnsureCorrectConcurrencyAsync(async () => isList
                ? await respository.NewAsync(new ViewContext(collection.Alias, parent), collection.EntityVariant.Type)
                : await respository.GetByIdAsync(parent!.Entity.Id!, new ViewContext(collection.Alias, parent))
                    ?? throw new InvalidOperationException($"Failed to get detail entity for given alias ({alias}) -- a detail entity should always exist."));

            var canEdit = (isList && isListEditor && await _authService.IsUserAuthorizedAsync(Operations.Update, entity)) || 
                (isDetails && await _authService.IsUserAuthorizedAsync(Operations.Update, parent!.Entity));
            var canView = isList && isListView && await _authService.IsUserAuthorizedAsync(Operations.Read, entity);

            if (!canEdit && !canView)
            {
                return TreeCollectionUI.None;
            }

            var tree = new TreeCollectionUI(collection.Alias, collection.RepositoryAlias, collection.Name)
            {
                EntitiesVisible = collection.TreeView?.EntityVisibility == EntityVisibilty.Visible,
                RootVisible = collection.TreeView?.RootVisibility == CollectionRootVisibility.Visible,
                Icon = collection.Icon ?? "Database",
                Color = collection.Color,
                DefaultOpenEntities = collection.TreeView?.DefaultOpenEntities ?? false
            };

            if (canEdit)
            {
                if (isList)
                {
                    tree.State = new PageStateModel
                    {
                        CollectionAlias = collection.Alias,
                        PageType = PageType.Collection,
                        ParentPath = parentPath,
                        UsageType = UsageType.Edit | ((parentPath != null) ? UsageType.NotRoot : UsageType.Root)
                    };
                }
                else if (isDetails)
                {
                    var entityVariant = collection.GetEntityVariant(entity);

                    tree.State = new PageStateModel
                    {
                        CollectionAlias = collection.Alias,
                        Id = parent!.Entity.Id,
                        PageType = PageType.Node,
                        ParentPath = parentPath,
                        UsageType = UsageType.Edit,
                        VariantAlias = entityVariant.Alias
                    };
                }
                
            }
            else if (canView)
            {
                tree.State = new PageStateModel
                {
                    CollectionAlias = collection.Alias,
                    PageType = PageType.Collection,
                    ParentPath = parentPath,
                    UsageType = UsageType.View | ((parentPath != null) ? UsageType.NotRoot : UsageType.Root)
                };
            }

            return tree;
        }

        public async Task<TreePageUI?> GetPageAsync(string alias)
        {
            var page = await _pageResolver.ResolveSetupAsync(alias);
            if (page == null)
            {
                throw new InvalidOperationException($"Failed to get page for given alias ({alias}).");
            }

            return new TreePageUI(page.Name, page.Icon, page.Color, new PageStateModel
            {
                CollectionAlias = page.Alias,
                PageType = PageType.Page
            });
        }

        public async Task<TreeNodesUI?> GetNodesAsync(string alias, ParentPath? parentPath, int pageNr, int pageSize)
        {
            var x = _httpContextAccessor.HttpContext;

            var collection = await _collectionResolver.ResolveSetupAsync(alias);
            if (collection == null)
            {
                throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            if (collection.TreeView?.EntityVisibility == EntityVisibilty.Visible)
            {
                var view = View.Create(pageSize, pageNr, default, default, collectionAlias: alias);
                var respository = _repositoryResolver.GetRepository(collection);
                var entities = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => respository.GetAllAsync(new ViewContext(collection.Alias, parent), view));

                var list = await entities.SelectNotNullAsync(async entity =>
                {
                    var canEdit = collection.NodeEditor != null && await _authService.IsUserAuthorizedAsync(Operations.Update, entity);
                    var canView = collection.ListView != null && await _authService.IsUserAuthorizedAsync(Operations.Read, entity);

                    if (!canEdit && !canView)
                    {
                        return null;
                    }

                    var entityVariant = collection.GetEntityVariant(entity);

                    var node = new TreeNodeUI(
                        entity.Id!,
                        collection.RepositoryAlias,
                        collection.TreeView.Name!.StringGetter.Invoke(entity),
                        collection.Collections.ToList(subCollection => (subCollection.Alias, PageType.Collection)))
                    {
                        RootVisibleOfCollections = collection.Collections.All(subCollection => subCollection.RootVisibility == CollectionRootVisibility.Visible),
                        DefaultOpenCollections = collection.TreeView?.DefaultOpenCollections ?? false
                    };

                    if (canEdit)
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
                    else if (canView)
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
                }).ToListAsync();

                return new TreeNodesUI(list.Take(pageSize).ToList(), view.MoreDataAvailable);
            }
            else
            {
                return default;
            }
        }

        public async Task<TreeRootUI> GetRootAsync()
        {
            var collections = (await _treeElementResolver.ResolveSetupAsync())
                .ToList(x => (x.Alias, x.Type));

            return new TreeRootUI("-1", AliasHelper.GetRepositoryAlias(typeof(object)), _cms.SiteName, collections)
            {
                State = new PageStateModel
                {
                    PageType = PageType.Dashboard
                }
            };
        }
    }
}
