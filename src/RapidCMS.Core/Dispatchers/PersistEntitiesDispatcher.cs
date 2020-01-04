using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
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


            var collection = _collectionResolver.GetCollection(request.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var button = collection.FindButton(actionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, parent);
            var entity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.NewAsync(parent, collection.EntityVariant.Type));

            await EnsureAuthorizedUserAsync(rootEditContext, button);

            ViewCommand viewCommand;

            var context = new ButtonContext(parent, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(rootEditContext, context))
            {
                case CrudType.Create:
                    if (button.EntityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must an {nameof(button.EntityVariant)}.");
                    }
                    if (usageType.HasFlag(UsageType.List))
                    {
                        viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, button.EntityVariant, parentPath, null) };
                    }
                    else
                    {
                        viewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = collectionAlias,
                            VariantAlias = button.EntityVariant.Alias,
                            ParentPath = parentPath?.ToPathString(),
                            Id = null
                        };
                    }
                    break;

                case CrudType.Update:
                    var contextsToProcess = editContexts.Where(x => x.IsModified()).Where(x => button.RequiresValidForm(x) ? x.IsValid() : true);
                    var affectedEntities = new List<IEntity>();
                    foreach (var editContext in contextsToProcess)
                    {
                        try
                        {
                            await EnsureAuthorizedUserAsync(editContext, button);
                            EnsureValidEditContext(editContext, button);
                            await EnsureCorrectConcurrencyAsync(() => collection.Repository.UpdateAsync(editContext));
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
                    viewCommand = new NavigateCommand { Uri = UriHelper.Collection(Constants.Edit, collectionAlias, parentPath) };
                    break;

                case CrudType.Up:
                    var (newParentPath, parentCollectionAlias, parentId) = ParentPath.RemoveLevel(parentPath);

                    if (parentCollectionAlias == null)
                    {
                        return new NoOperationCommand();
                    }

                    var parentCollection = _collectionProvider.GetCollection(parentCollectionAlias);

                    viewCommand = new NavigateCommand
                    {
                        Uri = UriHelper.Node(
                        usageType.HasFlag(UsageType.Edit) ? Constants.Edit : Constants.List,
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

            return viewCommand;
        }
    }
}
