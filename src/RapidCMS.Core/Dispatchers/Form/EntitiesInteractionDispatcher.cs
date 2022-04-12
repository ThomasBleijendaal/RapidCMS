using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Dispatchers.Form
{
    internal class EntitiesInteractionDispatcher :
        IInteractionDispatcher<PersistEntitiesRequestModel, ListViewCommandResponseModel>,
        IInteractionDispatcher<PersistEntitiesRequestModel, ListEditorCommandResponseModel>
    {
        private readonly INavigationStateProvider _navigationStateProvider;
        private readonly ISetupResolver<CollectionSetup> _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IButtonInteraction _buttonInteraction;
        private readonly IEditContextFactory _editContextFactory;
        private readonly IMediator _mediator;

        public EntitiesInteractionDispatcher(
            INavigationStateProvider navigationStateProvider,
            ISetupResolver<CollectionSetup> collectionResolver,
            IRepositoryResolver repositoryResolver,
            IConcurrencyService concurrencyService,
            IButtonInteraction buttonInteraction,
            IEditContextFactory editContextFactory,
            IMediator mediator)
        {
            _navigationStateProvider = navigationStateProvider;
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _concurrencyService = concurrencyService;
            _buttonInteraction = buttonInteraction;
            _editContextFactory = editContextFactory;
            _mediator = mediator;
        }

        Task<ListViewCommandResponseModel> IInteractionDispatcher<PersistEntitiesRequestModel, ListViewCommandResponseModel>.InvokeAsync(PersistEntitiesRequestModel request)
        {
            return InvokeAsync(request, new ListViewCommandResponseModel());
        }

        Task<ListEditorCommandResponseModel> IInteractionDispatcher<PersistEntitiesRequestModel, ListEditorCommandResponseModel>.InvokeAsync(PersistEntitiesRequestModel request)
        {
            return InvokeAsync(request, new ListEditorCommandResponseModel());
        }

        private async Task<T> InvokeAsync<T>(PersistEntitiesRequestModel request, T response)
            where T : ViewCommandResponseModel
        {
            var collection = await _collectionResolver.ResolveSetupAsync(request.ListContext.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var (crudType, entityVariant) = await _buttonInteraction.ValidateButtonInteractionAsync(request);

            switch (crudType)
            {
                case CrudType.Create:
                    if (entityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must have an EntityVariant.");
                    }

                    if (response is ListViewCommandResponseModel || ShouldFallbackToNavigatingToNodeEditor(collection))
                    {
                        _navigationStateProvider.AppendNavigationState(
                            request.NavigationState,
                            new NavigationState(
                                request.ListContext.CollectionAlias,
                                request.ListContext.Parent?.GetParentPath(),
                                entityVariant.Alias,
                                request.Related,
                                UsageType.New));
                    }
                    else
                    {
                        _navigationStateProvider.AppendNavigationState(
                            request.NavigationState,
                            new NavigationState(
                                request.ListContext.CollectionAlias,
                                request.ListContext.Parent?.GetParentPath(),
                                entityVariant.Alias,
                                request.Related,
                                UsageType.New,
                                PageType.Collection)
                            {
                                CollectionState = request.NavigationState.CollectionState
                            });
                    }
                    break;

                case CrudType.Update:
                    var affectedEntities = new List<IEntity>();

                    foreach (var editContext in request.ListContext.EditContexts.Where(f => f.IsModified() || f.IsReordered()))
                    {
                        var innerRequest = new PersistEntityCollectionRequestModel
                        {
                            ActionId = request.ActionId,
                            CustomData = request.CustomData,
                            EditContext = editContext,
                            ListContext = request.ListContext
                        };
                        if (!await editContext.IsValidAsync())
                        {
                            throw new InvalidEntityException();
                        }

                        await _buttonInteraction.ValidateButtonInteractionAsync(innerRequest);

                        if (editContext.IsModified())
                        {
                            var wrapper = await _editContextFactory.GetEditContextWrapperAsync(editContext);
                            await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(wrapper));
                        }
                        if (editContext.IsReordered())
                        {
                            await _concurrencyService.EnsureCorrectConcurrencyAsync(
                                () => repository.ReorderAsync(editContext.ReorderedBeforeId, editContext.Entity.Id!, new ViewContext(null, editContext.Parent)));
                        }

                        affectedEntities.Add(editContext.Entity);
                    }

                    response.RefreshIds = affectedEntities.SelectNotNull(x => x.Id);

                    _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                        collection.Alias,
                        collection.RepositoryAlias,
                        request.ListContext.ProtoEditContext.Parent?.GetParentPath(),
                        response.RefreshIds,
                        CrudType.Update));
                    break;

                case CrudType.None:
                    response.NoOp = true;
                    break;

                case CrudType.Refresh:
                    break;

                case CrudType.Return:
                    if (!_navigationStateProvider.RemoveNavigationState(request.NavigationState))
                    {
                        var parentPath = request.ListContext.Parent?.GetParentPath();
                        _navigationStateProvider.AppendNavigationState(
                            request.NavigationState,
                            new NavigationState(
                                request.ListContext.CollectionAlias,
                                parentPath,
                                null,
                                request.Related,
                                collection.ListEditor == null ? UsageType.View : UsageType.Edit,
                                PageType.Collection));
                    }
                    break;

                case CrudType.Up:
                    var (newParentPath, repositoryAlias, parentId) = ParentPath.RemoveLevel(request.ListContext.Parent?.GetParentPath());

                    if (repositoryAlias == null)
                    {
                        break;
                    }

                    var parentCollection = collection.Parent != null && collection.Parent.Type == PageType.Collection ? await _collectionResolver.ResolveSetupAsync(collection.Parent.Alias) : default;
                    if (parentCollection == null)
                    {
                        throw new InvalidOperationException("Cannot go Up on collection that is root.");
                    }

                    _navigationStateProvider.AppendNavigationState(
                        request.NavigationState,
                        new NavigationState(
                            request.ListContext.CollectionAlias,
                            newParentPath,
                            parentCollection.EntityVariant.Alias,
                            parentId,
                            collection.ListEditor == null ? UsageType.View : UsageType.Edit));
                    break;

                case CrudType.Add when request.Related != null:
                    _navigationStateProvider.AppendNavigationState(
                        request.NavigationState,
                        new NavigationState(
                            request.ListContext.CollectionAlias,
                            request.Related,
                            UsageType.Add));
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await _buttonInteraction.CompleteButtonInteractionAsync(request);

            return response;
        }

        private static bool ShouldFallbackToNavigatingToNodeEditor(CollectionSetup collection)
        {
            return collection.NodeEditor != null;
        }
    }
}
