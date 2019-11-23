using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Authorization;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Providers;

namespace RapidCMS.Common.Services
{
    internal class TreeService : ITreeService
    {
        private readonly ICollectionProvider _collectionProvider;
        private readonly IMetadataProvider _metadataProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IParentService _parentService;

        public TreeService(
            ICollectionProvider collectionProvider, 
            IMetadataProvider metadataProvider, 
            IHttpContextAccessor httpContextAccessor, 
            IAuthorizationService authorizationService,
            IParentService parentService)
        {
            _collectionProvider = collectionProvider;
            _metadataProvider = metadataProvider;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
            _parentService = parentService;
        }

        public async Task<TreeUI?> GetTreeAsync(string alias, ParentPath? parentPath)
        {
            var collection = _collectionProvider.GetCollection(alias);
            if (collection == null)
            {
                throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            var testEntity = await collection.Repository.InternalNewAsync(parent, collection.EntityVariant.Type);

            var viewAuthorizationChallenge = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                testEntity,
                Operations.Read);

            var editAuthorizationChallenge = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                testEntity,
                Operations.Update);

            var tree = new TreeUI
            {
                Alias = collection.Alias,
                Name = collection.Name,
                EntitiesVisible = collection.TreeView?.EntityVisibility == EntityVisibilty.Visible,
                RootVisible = collection.TreeView?.RootVisibility == CollectionRootVisibility.Visible,
                Icon = "list"
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

        public async Task<List<TreeNodeUI>> GetNodesAsync(string alias, ParentPath? parentPath)
        {
            var collection = _collectionProvider.GetCollection(alias);
            if (collection == null)
            {
                throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            if (collection.TreeView?.EntityVisibility == EntityVisibilty.Visible)
            {
                // TODO: pagination
                var query = Query.TakeElements(25);
                var entities = await collection.Repository.InternalGetAllAsync(parent, query);

                return await entities.ToListAsync(async entity =>
                {
                    var entityVariant = collection.GetEntityVariant(entity);

                    var node = new TreeNodeUI
                    {
                        Id = entity.Id,
                        Name = collection.TreeView.Name!.StringGetter.Invoke(entity),
                        RootVisibleOfCollections = collection.Collections.All(subCollection => subCollection.TreeView?.RootVisibility == CollectionRootVisibility.Visible),
                        Collections = collection.Collections.ToList(subCollection => subCollection.Alias)
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
            }
            else
            {
                return new List<TreeNodeUI>();
            }
        }

        public IDisposable SubscribeToUpdates(string alias, Func<Task> asyncCallback)
        {
            var collection = _collectionProvider.GetCollection(alias);
            return collection.Repository.ChangeToken.RegisterChangeCallback((x) => asyncCallback.Invoke(), null);
        }

        public TreeRootUI GetRoot()
        {
            var collections = _collectionProvider.GetAllCollections().Select(x => x.Alias).ToList();

            return new TreeRootUI
            {
                Collections = collections,
                Name = _metadataProvider.SiteName
            };
        }
    }
}
