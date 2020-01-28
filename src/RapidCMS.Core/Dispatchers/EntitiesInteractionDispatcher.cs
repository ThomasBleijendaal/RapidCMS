using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Dispatchers
{
    internal class EntitiesInteractionDispatcher :
        IInteractionDispatcher<PersistEntitiesRequestModel, ListViewCommandResponseModel>,
        IInteractionDispatcher<PersistEntitiesRequestModel, ListEditorCommandResponseModel>
    {
        private readonly ICollectionResolver _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IButtonInteraction _buttonInteraction;

        public EntitiesInteractionDispatcher(
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

        Task<ListViewCommandResponseModel> IInteractionDispatcher<PersistEntitiesRequestModel, ListViewCommandResponseModel>.InvokeAsync(PersistEntitiesRequestModel request, IPageState pageState)
        {
            return InvokeAsync(request, new ListViewCommandResponseModel(), pageState);
        }

        Task<ListEditorCommandResponseModel> IInteractionDispatcher<PersistEntitiesRequestModel, ListEditorCommandResponseModel>.InvokeAsync(PersistEntitiesRequestModel request, IPageState pageState)
        {
            return InvokeAsync(request, new ListEditorCommandResponseModel(), pageState);
        }

        private async Task<T> InvokeAsync<T>(PersistEntitiesRequestModel request, T response, IPageState pageState)
            where T : ViewCommandResponseModel
        {
            var collection = _collectionResolver.GetCollection(request.ListContext.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var (crudType, entityVariant) = await _buttonInteraction.ValidateButtonInteractionAsync(request);

            switch (crudType)
            {
                case CrudType.Create:
                    if (entityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must have an EntityVariant.");
                    }
                    if (response is ListViewCommandResponseModel)
                    {
                        pageState.PushState(new PageStateModel
                        {
                            PageType = PageType.Node,
                            UsageType = UsageType.New,

                            CollectionAlias = request.ListContext.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = request.ListContext.Parent?.GetParentPath()
                        });
                    }
                    else
                    {
                        var currentState = pageState.GetCurrentState();

                        pageState.PushState(new PageStateModel
                        {
                            PageType = PageType.Collection,
                            UsageType = UsageType.New,

                            CollectionAlias = request.ListContext.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = request.ListContext.Parent?.GetParentPath(),

                            // is this the best place here?
                            ActiveTab = currentState?.ActiveTab,
                            CurrentPage = currentState?.CurrentPage ?? 1,
                            MaxPage = currentState?.MaxPage,
                            SearchTerm = currentState?.SearchTerm,
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

                        await _buttonInteraction.ValidateButtonInteractionAsync(innerRequest);

                        if (editContext.IsModified())
                        {
                            await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(editContext));
                        }
                        if (editContext.IsReordered())
                        {
                            await _concurrencyService.EnsureCorrectConcurrencyAsync(
                                () => repository.ReorderAsync(editContext.ReorderedBeforeId, editContext.Entity.Id!, editContext.Parent));
                        }

                        affectedEntities.Add(editContext.Entity);
                    }

                    response.RefreshIds = affectedEntities.SelectNotNull(x => x.Id);
                    break;

                case CrudType.None:
                    response.NoOp = true;
                    break;

                case CrudType.Refresh:
                    break;

                case CrudType.Return:
                    if (pageState.PopState() == null)
                    {
                        pageState.ReplaceState(new PageStateModel
                        {
                            PageType = PageType.Collection,
                            UsageType = collection.ListEditor == null ? UsageType.View : UsageType.Edit,

                            CollectionAlias = request.ListContext.CollectionAlias,
                            ParentPath = request.ListContext.Parent?.GetParentPath()
                        });
                    }
                    break;

                case CrudType.Up:
                    if (pageState.PopState() == null)
                    {
                        var (newParentPath, parentCollectionAlias, parentId) = ParentPath.RemoveLevel(request.ListContext.Parent?.GetParentPath());

                        if (parentCollectionAlias == null)
                        {
                            break;
                        }

                        var parentCollection = _collectionResolver.GetCollection(parentCollectionAlias);

                        pageState.ReplaceState(new PageStateModel
                        {
                            PageType = PageType.Node,
                            UsageType = parentCollection.NodeEditor == null ? UsageType.View : UsageType.Edit,

                            CollectionAlias = parentCollectionAlias,
                            ParentPath = newParentPath,
                            VariantAlias = collection.EntityVariant.Alias,
                            Id = parentId
                        });
                    }
                    break;

                case CrudType.Add when request.Related != null:
                    pageState.PushState(new PageStateModel
                    {
                        PageType = PageType.Collection,
                        UsageType = UsageType.Add,

                        CollectionAlias = request.ListContext.CollectionAlias,
                        Related = request.Related
                    });
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await _buttonInteraction.CompleteButtonInteractionAsync(request);

            return response;
        }
    }
}
