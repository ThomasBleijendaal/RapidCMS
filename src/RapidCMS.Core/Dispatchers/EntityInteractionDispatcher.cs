using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Dispatchers
{
    internal class EntityInteractionDispatcher :
        IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel>,
        IInteractionDispatcher<PersistEntityRequestModel, NodeInListViewCommandResponseModel>,
        IInteractionDispatcher<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>
    {
        private readonly ICollectionResolver _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
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
            var collection = _collectionResolver.GetCollection(request.EditContext.CollectionAlias);
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
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(request.EditContext));

                    if (request.EditContext.IsReordered())
                    {
                        await _concurrencyService.EnsureCorrectConcurrencyAsync(
                            () => repository.ReorderAsync(request.EditContext.ReorderedBeforeId, request.EditContext.Entity.Id!, request.EditContext.Parent));
                    }

                    response.RefreshIds = new[] { request.EditContext.Entity.Id! };
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
                    
                    break;

                case CrudType.Pick when request is PersistRelatedEntityRequestModel relationRequest:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(relationRequest.Related, request.EditContext.Entity.Id!));

                    break;

                case CrudType.Remove when request is PersistRelatedEntityRequestModel relationRequest:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.RemoveAsync(relationRequest.Related, request.EditContext.Entity.Id!));

                    break;

                case CrudType.None:
                    response.NoOp = true;
                    break;

                case CrudType.Refresh:
                    break;

                case CrudType.Up:
                    if (pageState.PopState() == null)
                    {
                        pageState.ReplaceState(new PageStateModel
                        {
                            PageType = PageType.Collection,
                            UsageType = collection.ListEditor == null ? UsageType.View : UsageType.Edit,

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
