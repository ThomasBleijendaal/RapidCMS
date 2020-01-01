using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Resolvers;

namespace RapidCMS.Core.Services.Tree
{
    internal class TreeService : ITreeService
    {
        private readonly ICollections _setup;
        private readonly ICms _cms;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IParentService _parentService;

        public TreeService(
            ICollections setup,
            ICms cms,
            IRepositoryResolver repositoryResolver,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService,
            IParentService parentService)
        {
            _setup = setup;
            _cms = cms;
            _repositoryResolver = repositoryResolver;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
            _parentService = parentService;
        }

        public async Task<TreeUI?> GetTreeAsync(string alias, ParentPath? parentPath)
        {
            var collection = _setup.GetCollection(alias);
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

            var tree = new TreeUI
            {
                Alias = collection.Alias,
                Name = collection.Name,
                EntitiesVisible = collection.TreeView?.EntityVisibility == EntityVisibilty.Visible,
                RootVisible = collection.TreeView?.RootVisibility == CollectionRootVisibility.Visible,
                Icon = collection.Icon ?? "list"
            };

            // TODO
            //if (collection.ListEditor != null && editAuthorizationChallenge.Succeeded)
            //{
            //    tree.Path = UriHelper.Collection(Constants.Edit, collection.Alias, parentPath);
            //}
            //else if (collection.ListView != null && viewAuthorizationChallenge.Succeeded)
            //{
            //    tree.Path = UriHelper.Collection(Constants.List, collection.Alias, parentPath);
            //}

            return tree;
        }

        public async Task<List<TreeNodeUI>> GetNodesAsync(string alias, ParentPath? parentPath)
        {
            var collection = _setup.GetCollection(alias);
            if (collection == null)
            {
                throw new InvalidOperationException($"Failed to get collection for given alias ({alias}).");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            if (collection.TreeView?.EntityVisibility == EntityVisibilty.Visible)
            {
                // TODO: pagination
                var query = Query.TakeElements(25);
                var entities = await _repositoryResolver.GetRepository(collection).GetAllAsync(parent, query);

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
                        // TODO
                        //node.Path = UriHelper.Node(Constants.Edit, collection.Alias, entityVariant, parentPath, entity.Id);
                    }
                    else
                    {
                        var viewAuthorizationChallenge = await _authorizationService.AuthorizeAsync(
                            _httpContextAccessor.HttpContext.User,
                            entity,
                            Operations.Read);

                        if (viewAuthorizationChallenge.Succeeded)
                        {
                            // TODO
                            //node.Path = UriHelper.Node(Constants.View, collection.Alias, entityVariant, parentPath, entity.Id);
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
            return _repositoryResolver.GetRepository(alias).ChangeToken.RegisterChangeCallback((x) => asyncCallback.Invoke(), null);
        }

        public TreeRootUI GetRoot()
        {
            var collections = _setup.GetRootCollections().Select(x => x.Alias).ToList();

            return new TreeRootUI
            {
                Collections = collections,
                Name = _cms.SiteName
            };
        }
    }
}
