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
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Services.Tree
{
    internal class TreeService : ITreeService
    {
        private readonly ISetupResolver<CollectionSetup> _collectionResolver;
        private readonly ISetupResolver<PageSetup> _pageResolver;
        private readonly ISetupResolver<IEnumerable<TreeElementSetup>> _treeElementResolver;
        private readonly ICms _cms;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IAuthService _authService;
        private readonly IParentService _parentService;
        private readonly IConcurrencyService _concurrencyService;

        public TreeService(
            ISetupResolver<CollectionSetup> collectionResolver,
            ISetupResolver<PageSetup> pageResolver,
            ISetupResolver<IEnumerable<TreeElementSetup>> treeElementResolver,
            ICms cms,
            IRepositoryResolver repositoryResolver,
            IAuthService authService,
            IParentService parentService,
            IConcurrencyService concurrencyService)
        {
            _collectionResolver = collectionResolver;
            _pageResolver = pageResolver;
            _treeElementResolver = treeElementResolver;
            _cms = cms;
            _repositoryResolver = repositoryResolver;
            _authService = authService;
            _parentService = parentService;
            _concurrencyService = concurrencyService;
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
                    tree.NavigateTo = new NavigationState(
                        collection.Alias, 
                        parentPath,
                        UsageType.Edit);
                }
                else if (isDetails)
                {
                    var entityVariant = collection.GetEntityVariant(entity);

                    tree.NavigateTo = new NavigationState(
                        collection.Alias, parentPath, 
                        entityVariant.Alias, 
                        parent!.Entity.Id, 
                        UsageType.Edit);
                }
                
            }
            else if (canView)
            {
                tree.NavigateTo = new NavigationState(
                    collection.Alias,
                    parentPath,
                    UsageType.View);
            }

            return tree;
        }

        public async Task<TreePageUI?> GetPageAsync(string alias, ParentPath? parentPath)
        {
            var page = await _pageResolver.ResolveSetupAsync(alias);
            if (page == null)
            {
                throw new InvalidOperationException($"Failed to get page for given alias ({alias}).");
            }

            return new TreePageUI(page.Name, page.Icon, page.Color, new NavigationState(page.Alias, parentPath));
        }

        public async Task<TreeNodesUI?> GetNodesAsync(string alias, ParentPath? parentPath, int pageNr, int pageSize)
        {
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
                        collection.Collections.ToList(subCollection => (subCollection.Alias, subCollection.Type)))
                    {
                        RootVisibleOfCollections = collection.Collections.All(subCollection => subCollection.RootVisibility == CollectionRootVisibility.Visible),
                        DefaultOpenCollections = collection.TreeView?.DefaultOpenCollections ?? false
                    };

                    if (canEdit)
                    {
                        node.NavigateTo = new NavigationState(
                            collection.Alias,
                            parentPath,
                            entityVariant.Alias,
                            entity.Id,
                            UsageType.Edit);
                    }
                    else if (canView)
                    {
                        node.NavigateTo = new NavigationState(
                            collection.Alias,
                            parentPath,
                            entityVariant.Alias,
                            entity.Id,
                            UsageType.View);
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
                NavigateTo = new NavigationState()
            };
        }
    }
}
