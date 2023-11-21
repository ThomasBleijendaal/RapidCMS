using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Dispatchers.Form;

internal class EntityInteractionDispatcher :
    IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel>,
    IInteractionDispatcher<PersistEntityRequestModel, NodeInListViewCommandResponseModel>,
    IInteractionDispatcher<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>
{
    private readonly INavigationStateProvider _navigationStateProvider;
    private readonly ISetupResolver<CollectionSetup> _collectionResolver;
    private readonly IRepositoryResolver _repositoryResolver;
    private readonly IConcurrencyService _concurrencyService;
    private readonly IButtonInteraction _buttonInteraction;
    private readonly IEditContextFactory _editContextFactory;
    private readonly IMediator _mediator;

    public EntityInteractionDispatcher(
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

    Task<NodeViewCommandResponseModel> IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel>.InvokeAsync(PersistEntityRequestModel request)
    {
        return InvokeAsync(request, new NodeViewCommandResponseModel());
    }

    Task<NodeInListViewCommandResponseModel> IInteractionDispatcher<PersistEntityRequestModel, NodeInListViewCommandResponseModel>.InvokeAsync(PersistEntityRequestModel request)
    {
        return InvokeAsync(request, new NodeInListViewCommandResponseModel());
    }

    Task<NodeInListViewCommandResponseModel> IInteractionDispatcher<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>.InvokeAsync(PersistRelatedEntityRequestModel request)
    {
        return InvokeAsync(request, new NodeInListViewCommandResponseModel());
    }

    private async Task<T> InvokeAsync<T>(PersistEntityRequestModel request, T response)
        where T : ViewCommandResponseModel
    {
        var collection = await _collectionResolver.ResolveSetupAsync(request.EditContext.CollectionAlias);
        var repository = _repositoryResolver.GetRepository(collection);

        var entityVariant = collection.GetEntityVariant(request.EditContext.Entity);

        var crudType = await _buttonInteraction.ValidateButtonInteractionAsync(request);

        switch (crudType)
        {
            case CrudType.View:
                _navigationStateProvider.AppendNavigationState(request.NavigationState, new NavigationState(
                    request.EditContext.CollectionAlias,
                    request.EditContext.Parent?.GetParentPath(),
                    entityVariant.Alias,
                    request.EditContext.Entity.Id,
                    UsageType.View));
                break;

            case CrudType.Edit:
                _navigationStateProvider.AppendNavigationState(request.NavigationState, new NavigationState(
                    request.EditContext.CollectionAlias,
                    request.EditContext.Parent?.GetParentPath(),
                    entityVariant.Alias,
                    request.EditContext.Entity.Id,
                    UsageType.Edit));
                break;

            case CrudType.Update:
                var updateContext = await _editContextFactory.GetEditContextWrapperAsync(request.EditContext);
                if (!await updateContext.IsValidAsync())
                {
                    throw new InvalidEntityException();
                }

                await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(updateContext));

                if (request.EditContext.IsReordered())
                {
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(
                        () => repository.ReorderAsync(request.EditContext.ReorderedBeforeId, request.EditContext.Entity.Id!, new ViewContext(collection.Alias, request.EditContext.Parent)));
                }

                response.RefreshIds = new[] { request.EditContext.Entity.Id! };

                _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                    collection.Alias,
                    collection.RepositoryAlias,
                    request.EditContext.Parent?.GetParentPath(),
                    request.EditContext.Entity.Id,
                    CrudType.Update));

                break;

            case CrudType.Insert:
                var insertContext = await _editContextFactory.GetEditContextWrapperAsync(request.EditContext);
                if (!await insertContext.IsValidAsync())
                {
                    throw new InvalidEntityException();
                }

                var newEntity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.InsertAsync(insertContext));
                if (newEntity == null)
                {
                    // add another
                    break;
                }

                if (request is PersistRelatedEntityRequestModel related)
                {
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(new RelatedViewContext(related.Related, collection.Alias, default), request.EditContext.Entity.Id!));
                }

                if (response is NodeViewCommandResponseModel)
                {
                    _navigationStateProvider.AppendNavigationState(request.NavigationState, new NavigationState(
                        request.EditContext.CollectionAlias,
                        request.EditContext.Parent?.GetParentPath(),
                        entityVariant.Alias,
                        newEntity.Id,
                        UsageType.Edit));
                }

                _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                    collection.Alias,
                    collection.RepositoryAlias,
                    request.EditContext.Parent?.GetParentPath(),
                    request.EditContext.Entity.Id,
                    CrudType.Insert));

                break;

            case CrudType.Delete:
                await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.DeleteAsync(request.EditContext.Entity.Id!, new ViewContext(collection.Alias, request.EditContext.Parent)));

                if (response is NodeViewCommandResponseModel)
                {
                    _navigationStateProvider.ReplaceNavigationState(request.NavigationState, new NavigationState(
                        request.EditContext.CollectionAlias,
                        request.EditContext.Parent?.GetParentPath(),
                        collection.ListEditor == null ? UsageType.View : UsageType.Edit));
                }

                _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                    collection.Alias,
                    collection.RepositoryAlias,
                    request.EditContext.Parent?.GetParentPath(),
                    request.EditContext.Entity.Id,
                    CrudType.Delete));

                break;

            case CrudType.Pick when request is PersistRelatedEntityRequestModel relationRequest:

                await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(new RelatedViewContext(relationRequest.Related, collection.Alias, default), request.EditContext.Entity.Id!));

                _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                    collection.Alias,
                    collection.RepositoryAlias,
                    request.EditContext.Parent?.GetParentPath(),
                    request.EditContext.Entity.Id,
                    CrudType.Pick));

                break;

            case CrudType.Remove when request is PersistRelatedEntityRequestModel relationRequest:

                await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.RemoveAsync(new RelatedViewContext(relationRequest.Related, collection.Alias, default), request.EditContext.Entity.Id!));

                _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                    collection.Alias,
                    collection.RepositoryAlias,
                    request.EditContext.Parent?.GetParentPath(),
                    request.EditContext.Entity.Id,
                    CrudType.Remove));

                break;

            case CrudType.None:
                response.NoOp = true;
                break;

            case CrudType.Refresh:
                break;

            case CrudType.Up:
                _navigationStateProvider.AppendNavigationState(request.NavigationState, new NavigationState(
                    request.EditContext.CollectionAlias,
                    request.EditContext.Parent?.GetParentPath(),
                    collection.ListEditor == null ? UsageType.View : UsageType.Edit));
                break;

            default:
                throw new InvalidOperationException();
        }

        await _buttonInteraction.CompleteButtonInteractionAsync(request);

        return response;
    }
}
