using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.NavigationState;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Dispatchers
{
    // TODO: refactor futher to pull the three dispatches from each other
    internal class EntityInteractionDispatcher :
        IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel>,
        IInteractionDispatcher<PersistEntityRequestModel, NodeInListViewCommandResponseModel>,
        IInteractionDispatcher<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>
    {
        private readonly ICollectionResolver _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private INavigationStateService _navigationStateService;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IButtonInteraction _buttonInteraction;

        public EntityInteractionDispatcher(
            ICollectionResolver collectionResolver,
            IRepositoryResolver repositoryResolver,
            IConcurrencyService concurrencyService,
            IButtonInteraction buttonInteraction)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _concurrencyService = concurrencyService;
            _buttonInteraction = buttonInteraction;
        }

        Task<NodeViewCommandResponseModel> IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel>.InvokeAsync(PersistEntityRequestModel request, INavigationStateService navigationState)
        {
            _navigationStateService = navigationState;
            return InvokeAsync(request, new NodeViewCommandResponseModel());
        }

        Task<NodeInListViewCommandResponseModel> IInteractionDispatcher<PersistEntityRequestModel, NodeInListViewCommandResponseModel>.InvokeAsync(PersistEntityRequestModel request, INavigationStateService navigationState)
        {
            _navigationStateService = navigationState;
            return InvokeAsync(request, new NodeInListViewCommandResponseModel());
        }

        Task<NodeInListViewCommandResponseModel> IInteractionDispatcher<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>.InvokeAsync(PersistRelatedEntityRequestModel request, INavigationStateService navigationState)
        {
            _navigationStateService = navigationState;
            return InvokeAsync(request, new NodeInListViewCommandResponseModel());
        }

        private async Task<T> InvokeAsync<T>(PersistEntityRequestModel request, T response)
            where T : ViewCommandResponseModel
        {
            var collection = _collectionResolver.GetCollection(request.EditContext.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var entityVariant = collection.GetEntityVariant(request.EditContext.Entity);

            var crudType = await _buttonInteraction.ValidateButtonInteractionAsync(request);

            switch (crudType)
            {
                case CrudType.View:
                    _navigationStateService.PushState(new NavigationStateModel
                    {
                        PageType = PageType.Node,
                        UsageType = UsageType.View,

                        CollectionAlias = request.EditContext.CollectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentPath = request.EditContext.Parent?.GetParentPath(),
                        Id = request.EditContext.Entity.Id
                    });
                    break;

                case CrudType.Edit:
                    _navigationStateService.PushState(new NavigationStateModel
                    {
                        PageType = PageType.Node,
                        UsageType = UsageType.Edit,

                        CollectionAlias = request.EditContext.CollectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentPath = request.EditContext.Parent?.GetParentPath(),
                        Id = request.EditContext.Entity.Id
                    });
                    break;

                case CrudType.Update:
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(request.EditContext));

                    if (request.EditContext.IsReordered())
                    {
                        await _concurrencyService.EnsureCorrectConcurrencyAsync(
                            () => repository.ReorderAsync(request.EditContext.ReorderedBeforeId, request.EditContext.Entity.Id!, request.EditContext.Parent));
                    }

                    response.ViewCommand = new ViewCommand
                    {
                        RefreshIds = new[] { request.EditContext.Entity.Id! }
                    };
                    break;

                case CrudType.Insert:
                    var newEntity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.InsertAsync(request.EditContext));
                    if (newEntity == null)
                    {
                        throw new Exception("Inserting the new entity failed.");
                    }

                    if (request is PersistRelatedEntityRequestModel related)
                    {
                        await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(related.Related, request.EditContext.Entity.Id!));
                    }

                    if (response is NodeViewCommandResponseModel)
                    {
                        _navigationStateService.ReplaceState(new NavigationStateModel
                        {
                            PageType = PageType.Node,
                            UsageType = UsageType.Edit,

                            CollectionAlias = request.EditContext.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = request.EditContext.Parent?.GetParentPath(),
                            Id = newEntity.Id
                        });
                    }
                    else if (response is NodeInListViewCommandResponseModel)
                    {
                        // what does this do?
                        response.ViewCommand = new ViewCommand
                        {
                            ReloadData = true
                        };
                    }

                    break;

                case CrudType.Delete:
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.DeleteAsync(request.EditContext.Entity.Id!, request.EditContext.Parent));

                    if (response is NodeViewCommandResponseModel)
                    {
                        if (_navigationStateService.PopState() == null)
                        {
                            _navigationStateService.ReplaceState(new NavigationStateModel
                            {
                                PageType = PageType.Collection,
                                UsageType = collection.ListEditor == null ? UsageType.List : UsageType.Edit,

                                CollectionAlias = request.EditContext.CollectionAlias,
                                ParentPath = request.EditContext.Parent?.GetParentPath()
                            });
                        }
                    }
                    else
                    {
                        response.ViewCommand = new ViewCommand
                        {
                            ReloadData = true
                        };
                    }

                    break;

                case CrudType.Pick when request is PersistRelatedEntityRequestModel relationRequest:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(relationRequest.Related, request.EditContext.Entity.Id!));

                    response.ViewCommand = new ViewCommand
                    {
                        ReloadData = true
                    };

                    break;

                case CrudType.Remove when request is PersistRelatedEntityRequestModel relationRequest:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.RemoveAsync(relationRequest.Related, request.EditContext.Entity.Id!));

                    response.ViewCommand = new ViewCommand
                    {
                        ReloadData = true
                    };

                    break;

                case CrudType.None:
                    break;

                case CrudType.Refresh:
                    response.ViewCommand = new ViewCommand
                    {
                        ReloadData = true
                    };
                    break;

                case CrudType.Up:
                    if (_navigationStateService.PopState() == null)
                    {
                        _navigationStateService.ReplaceState(new NavigationStateModel
                        {
                            PageType = PageType.Collection,
                            UsageType = collection.ListEditor == null ? UsageType.List : UsageType.Edit,

                            CollectionAlias = request.EditContext.CollectionAlias,
                            ParentPath = request.EditContext.Parent?.GetParentPath()
                        });
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await _buttonInteraction.CompleteButtonInteractionAsync(request);

            return response;
        }
    }
}
