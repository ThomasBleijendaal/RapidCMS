using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Dispatchers.Form
{
    internal class EntityInteractionDispatcher :
        IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel>,
        IInteractionDispatcher<PersistEntityRequestModel, NodeInListViewCommandResponseModel>,
        IInteractionDispatcher<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IButtonInteraction _buttonInteraction;
        private readonly IEditContextFactory _editContextFactory;
        private readonly IMediator _mediator;

        public EntityInteractionDispatcher(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IRepositoryResolver repositoryResolver,
            IConcurrencyService concurrencyService,
            IButtonInteraction buttonInteraction,
            IEditContextFactory editContextFactory,
            IMediator mediator)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _concurrencyService = concurrencyService;
            _buttonInteraction = buttonInteraction;
            _editContextFactory = editContextFactory;
            _mediator = mediator;
        }

        Task<NodeViewCommandResponseModel> IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel>.InvokeAsync(PersistEntityRequestModel request, IPageState pageState)
        {
            return InvokeAsync(request, new NodeViewCommandResponseModel(), pageState);
        }

        Task<NodeInListViewCommandResponseModel> IInteractionDispatcher<PersistEntityRequestModel, NodeInListViewCommandResponseModel>.InvokeAsync(PersistEntityRequestModel request, IPageState pageState)
        {
            return InvokeAsync(request, new NodeInListViewCommandResponseModel(), pageState);
        }

        Task<NodeInListViewCommandResponseModel> IInteractionDispatcher<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>.InvokeAsync(PersistRelatedEntityRequestModel request, IPageState pageState)
        {
            return InvokeAsync(request, new NodeInListViewCommandResponseModel(), pageState);
        }

        private async Task<T> InvokeAsync<T>(PersistEntityRequestModel request, T response, IPageState pageState)
            where T : ViewCommandResponseModel
        {
            var collection = _collectionResolver.ResolveSetup(request.EditContext.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var entityVariant = collection.GetEntityVariant(request.EditContext.Entity);

            var crudType = await _buttonInteraction.ValidateButtonInteractionAsync(request);

            switch (crudType)
            {
                case CrudType.View:
                    pageState.PushState(new PageStateModel
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
                    pageState.PushState(new PageStateModel
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
                    var updateContext = _editContextFactory.GetEditContextWrapper(request.EditContext);
                    if (!updateContext.IsValid())
                    {
                        throw new InvalidEntityException();
                    }

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(updateContext));

                    if (request.EditContext.IsReordered())
                    {
                        await _concurrencyService.EnsureCorrectConcurrencyAsync(
                            () => repository.ReorderAsync(request.EditContext.ReorderedBeforeId, request.EditContext.Entity.Id!, request.EditContext.Parent));
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
                    var insertContext = _editContextFactory.GetEditContextWrapper(request.EditContext);
                    if (!insertContext.IsValid())
                    {
                        throw new InvalidEntityException();
                    }

                    var newEntity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.InsertAsync(insertContext));
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
                        pageState.ReplaceState(new PageStateModel
                        {
                            PageType = PageType.Node,
                            UsageType = UsageType.Edit,

                            CollectionAlias = request.EditContext.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = request.EditContext.Parent?.GetParentPath(),
                            Id = newEntity.Id
                        });
                    }

                    _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                        collection.Alias,
                        collection.RepositoryAlias,
                        request.EditContext.Parent?.GetParentPath(),
                        request.EditContext.Entity.Id,
                        CrudType.Insert));

                    break;

                case CrudType.Delete:
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.DeleteAsync(request.EditContext.Entity.Id!, request.EditContext.Parent));

                    if (response is NodeViewCommandResponseModel)
                    {
                        if (pageState.PopState() == null)
                        {
                            pageState.ReplaceState(new PageStateModel
                            {
                                PageType = PageType.Collection,
                                UsageType = collection.ListEditor == null ? UsageType.List : UsageType.Edit,

                                CollectionAlias = request.EditContext.CollectionAlias,
                                ParentPath = request.EditContext.Parent?.GetParentPath()
                            });
                        }
                    }

                    _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                        collection.Alias,
                        collection.RepositoryAlias,
                        request.EditContext.Parent?.GetParentPath(),
                        request.EditContext.Entity.Id,
                        CrudType.Delete));

                    break;

                case CrudType.Pick when request is PersistRelatedEntityRequestModel relationRequest:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(relationRequest.Related, request.EditContext.Entity.Id!));

                    _mediator.NotifyEvent(this, new CollectionRepositoryEventArgs(
                        collection.Alias,
                        collection.RepositoryAlias,
                        request.EditContext.Parent?.GetParentPath(),
                        request.EditContext.Entity.Id,
                        CrudType.Pick));

                    break;

                case CrudType.Remove when request is PersistRelatedEntityRequestModel relationRequest:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.RemoveAsync(relationRequest.Related, request.EditContext.Entity.Id!));

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
                    if (pageState.PopState() == null)
                    {
                        var parentPath = request.EditContext.Parent?.GetParentPath();
                        pageState.ReplaceState(new PageStateModel
                        {
                            PageType = PageType.Collection,
                            UsageType = (collection.ListEditor == null ? UsageType.View : UsageType.Edit)
                             | ((parentPath != null) ? UsageType.NotRoot : UsageType.Root),

                            CollectionAlias = request.EditContext.CollectionAlias,
                            ParentPath = parentPath
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
