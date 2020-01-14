using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Dispatchers
{
    internal class EntitiesInteractionDispatcher : 
        IInteractionDispatcher<PersistChildEntitiesRequestModel, ListViewCommandResponseModel>
    {
        private readonly ICollectionResolver _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IButtonInteraction _buttonInteraction;
        private readonly IParentService _parentService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthService _authService;

        public EntitiesInteractionDispatcher(
            ICollectionResolver collectionResolver, 
            IRepositoryResolver repositoryResolver, 
            IConcurrencyService concurrencyService,
            IButtonInteraction buttonInteraction,
            IParentService parentService, 
            IAuthorizationService authorizationService,
            IServiceProvider serviceProvider,
            IAuthService authService)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _concurrencyService = concurrencyService;
            _buttonInteraction = buttonInteraction;
            _parentService = parentService;
            _authorizationService = authorizationService;
            _authService = authService;
        }
        
        Task<ListViewCommandResponseModel> IInteractionDispatcher<PersistChildEntitiesRequestModel, ListViewCommandResponseModel>.InvokeAsync(PersistChildEntitiesRequestModel request)
        {
            return InvokeAsync(request, new ListViewCommandResponseModel());
        }

        private async Task<T> InvokeAsync<T>(PersistEntitiesRequestModel request, T response)
            where T : ViewCommandResponseModel
        {
            var childRequest = request as PersistChildEntitiesRequestModel;

            var collection = _collectionResolver.GetCollection(request.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var (crudType, entityVariant) = await _buttonInteraction.ValidateButtonInteractionAsync(request);

            switch (crudType)
            {
                case CrudType.Create:
                    if (entityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must have an EntityVariant.");
                    }
                    if (request.UsageType.HasFlag(UsageType.List))
                    {
                        response.ViewCommand = new NavigateCommand
                        {
                            Uri = UriHelper.Node(
                                Constants.New,
                                request.CollectionAlias,
                                entityVariant,
                                childRequest?.ParentPath,
                                null)
                        };
                    }
                    else
                    {
                        response.ViewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = request.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = childRequest?.ParentPath?.ToPathString(),
                            Id = null
                        };
                    }
                    break;

                case CrudType.Update:
                    //var contextsToProcess = request.ListContext.EditContexts.Where(x => x.IsModified()).Where(x => button.RequiresValidForm(x) ? x.IsValid() : true);
                    //var affectedEntities = new List<IEntity>();
                    //foreach (var editContext in contextsToProcess)
                    //{
                    //    try
                    //    {
                    //        await _authService.EnsureAuthorizedUserAsync(editContext, button);
                    //        if (button.RequiresValidForm(editContext) && !editContext.IsValid())
                    //        {
                    //            throw new InvalidEntityException();
                    //        }
                    //        await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(editContext));
                    //        affectedEntities.Add(editContext.Entity);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        // do not care about any exception in this case
                    //    }
                    //}
                    //response.ViewCommand = new ReloadCommand(affectedEntities.SelectNotNull(x => x.Id));
                    break;

                case CrudType.None:
                    response.ViewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    response.ViewCommand = new ReloadCommand();
                    break;

                case CrudType.Return:
                    response.ViewCommand = new NavigateCommand
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
                        response.ViewCommand = new NoOperationCommand();
                        break;
                    }

                    var parentCollection = _collectionResolver.GetCollection(parentCollectionAlias);

                    response.ViewCommand = new NavigateCommand
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

            await _buttonInteraction.CompleteButtonInteractionAsync(request);

            return response;
        }

        
    }
}
