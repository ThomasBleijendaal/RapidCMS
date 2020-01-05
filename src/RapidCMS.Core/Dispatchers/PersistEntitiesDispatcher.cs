using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Dispatchers
{
    internal class PersistEntitiesDispatcher : BaseDispatcher, IDispatcher<PersistEntitiesRequestModel, ViewCommandResponseModel>
    {
        public PersistEntitiesDispatcher(ICollectionResolver collectionResolver, IRepositoryResolver repositoryResolver, IParentService parentService, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, SemaphoreSlim semaphore) : base(collectionResolver, repositoryResolver, parentService, authorizationService, httpContextAccessor, serviceProvider, semaphore)
        {
        }

        public async Task<ViewCommandResponseModel> InvokeAsync(PersistEntitiesRequestModel request)
        {
            var childRequest = request as PersistChildEntitiesRequestModel;

            var collection = _collectionResolver.GetCollection(request.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var button = collection.FindButton(request.ActionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {request.CollectionAlias}");
            }

            var parent = childRequest == null ? default : await _parentService.GetParentAsync(childRequest.ParentPath);

            var rootEditContext = await GetNewEditContextAsync(request.UsageType, request.CollectionAlias, parent);
            var entity = await EnsureCorrectConcurrencyAsync(() => repository.NewAsync(parent, collection.EntityVariant.Type));

            // TODO: this can cause an Update action be validated on the root, while it applies to the children (which is also checked)
            // this could lead to invalid rejection of action
            await EnsureAuthorizedUserAsync(rootEditContext, button);

            ViewCommand viewCommand;

            var context = new ButtonContext(parent, request.CustomData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(rootEditContext, context))
            {
                case CrudType.Create:
                    if (button.EntityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must an {nameof(button.EntityVariant)}.");
                    }
                    if (request.UsageType.HasFlag(UsageType.List))
                    {
                        viewCommand = new NavigateCommand
                        {
                            Uri = UriHelper.Node(
                                Constants.New,
                                request.CollectionAlias,
                                button.EntityVariant,
                                childRequest?.ParentPath,
                                null)
                        };
                    }
                    else
                    {
                        viewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = request.CollectionAlias,
                            VariantAlias = button.EntityVariant.Alias,
                            ParentPath = childRequest?.ParentPath?.ToPathString(),
                            Id = null
                        };
                    }
                    break;

                case CrudType.Update:
                    var contextsToProcess = request.EditContexts.Where(x => x.IsModified()).Where(x => button.RequiresValidForm(x) ? x.IsValid() : true);
                    var affectedEntities = new List<IEntity>();
                    foreach (var editContext in contextsToProcess)
                    {
                        try
                        {
                            await EnsureAuthorizedUserAsync(editContext, button);
                            EnsureValidEditContext(editContext, button);
                            await EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(editContext));
                            affectedEntities.Add(editContext.Entity);
                        }
                        catch (Exception)
                        {
                            // do not care about any exception in this case
                        }
                    }
                    viewCommand = new ReloadCommand(affectedEntities.SelectNotNull(x => x.Id));
                    break;

                case CrudType.None:
                    viewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Return:
                    viewCommand = new NavigateCommand
                    {
                        Uri = UriHelper.Collection(
                            Constants.Edit,
                            request.CollectionAlias,
                            childRequest?.ParentPath)
                    };
                    break;

                case CrudType.Up:
                    var (newParentPath, parentCollectionAlias, parentId) = ParentPath.RemoveLevel(childRequest?.ParentPath);

                    if (parentCollectionAlias == null)
                    {
                        viewCommand = new NoOperationCommand();
                        break;
                    }

                    var parentCollection = _collectionResolver.GetCollection(parentCollectionAlias);

                    viewCommand = new NavigateCommand
                    {
                        Uri = UriHelper.Node(
                        request.UsageType.HasFlag(UsageType.Edit) ? Constants.Edit : Constants.View,
                        parentCollectionAlias,
                        parentCollection.EntityVariant,
                        newParentPath,
                        parentId)
                    };
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await button.ButtonClickAfterRepositoryActionAsync(rootEditContext, context);

            return new ViewCommandResponseModel
            {
                ViewCommand = viewCommand
            };
        }
    }
}
