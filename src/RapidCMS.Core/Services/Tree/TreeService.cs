using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Services.Tree
{
    internal class TreeService : ITreeService
    {
        private readonly ICollectionResolver _collectionResolver;
        private readonly ICms _cms;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IParentService _parentService;

        public TreeService(
            ICollectionResolver collectionResolver,
            ICms cms,
            IRepositoryResolver repositoryResolver,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService,
            IParentService parentService)
        {
            _collectionResolver = collectionResolver;
            _cms = cms;
            _repositoryResolver = repositoryResolver;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
            _parentService = parentService;
        }

        public async Task<TreeCollectionUI?> GetCollectionAsync(string alias, ParentPath? parentPath)
        {
            var collection = _collectionResolver.GetCollection(alias);
            if (collection == null)
            {
                throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            var testEntity = await _repositoryResolver.GetRepository(collection).NewAsync(parent, collection.EntityVariant.Type);

            var viewAuthorizationChallenge = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                testEntity,
                Operations.Read);

            var editAuthorizationChallenge = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                testEntity,
                Operations.Update);

            var tree = new TreeCollectionUI(collection.Alias, collection.Name)
            {
                EntitiesVisible = collection.TreeView?.EntityVisibility == EntityVisibilty.Visible,
                RootVisible = collection.TreeView?.RootVisibility == CollectionRootVisibility.Visible,
                Icon = collection.Icon ?? "list"
            };

            if (collection.ListEditor != null && editAuthorizationChallenge.Succeeded)
            {
                tree.Path = UriHelper.Collection(Constants.Edit, collection.Alias, parentPath);
            }
            else if (collection.ListView != null && viewAuthorizationChallenge.Succeeded)
            {
                tree.Path = UriHelper.Collection(Constants.List, collection.Alias, parentPath);
            }

            return tree;
        }

        public async Task<TreeNodesUI?> GetNodesAsync(string alias, ParentPath? parentPath, int pageNr, int pageSize)
        {
            var collection = _collectionResolver.GetCollection(alias);
            if (collection == null)
            {
                throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            if (collection.TreeView?.EntityVisibility == EntityVisibilty.Visible)
            {
                var query = Query.Create(pageSize, pageNr, default, default);
                var entities = await _repositoryResolver.GetRepository(collection).GetAllAsync(parent, query);

                var list = await entities.ToListAsync(async entity =>
                {
                    var entityVariant = collection.GetEntityVariant(entity);

                    var node = new TreeNodeUI(
                        entity.Id!, 
                        collection.TreeView.Name!.StringGetter.Invoke(entity), 
                        collection.Collections.ToList(subCollection => subCollection.Alias))
                    {
                        RootVisibleOfCollections = collection.Collections.All(subCollection => subCollection.TreeView?.RootVisibility == CollectionRootVisibility.Visible),
                    };

                    var editAuthorizationChallenge = await _authorizationService.AuthorizeAsync(
                        _httpContextAccessor.HttpContext.User,
                        entity,
                        Operations.Update);

                    if (editAuthorizationChallenge.Succeeded)
                    {
                        node.Path = UriHelper.Node(Constants.Edit, collection.Alias, entityVariant, parentPath, entity.Id);
                    }
                    else
                    {
                        var viewAuthorizationChallenge = await _authorizationService.AuthorizeAsync(
                            _httpContextAccessor.HttpContext.User,
                            entity,
                            Operations.Read);

                        if (viewAuthorizationChallenge.Succeeded)
                        {
                            node.Path = UriHelper.Node(Constants.View, collection.Alias, entityVariant, parentPath, entity.Id);
                        }
                    }

                    return node;
                });

                return new TreeNodesUI(list.Take(pageSize).ToList(), query.MoreDataAvailable);
            }
            else
            {
                return default;
            }
        }

        public IDisposable SubscribeToUpdates(string alias, Func<Task> asyncCallback)
        {
            return _repositoryResolver.GetRepository(alias).ChangeToken.RegisterChangeCallback((x) => asyncCallback.Invoke(), null);
        }

        public TreeRootUI GetRoot()
        {
            var collections = _collectionResolver.GetRootCollections().Select(x => x.Alias).ToList();

            return new TreeRootUI("-1", _cms.SiteName, collections);
        }
    }
}
