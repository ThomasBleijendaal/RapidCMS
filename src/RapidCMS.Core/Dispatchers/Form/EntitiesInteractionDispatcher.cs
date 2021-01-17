using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Dispatchers.Form
{
    internal class EntitiesInteractionDispatcher :
        IInteractionDispatcher<PersistEntitiesRequestModel, ListViewCommandResponseModel>,
        IInteractionDispatcher<PersistEntitiesRequestModel, ListEditorCommandResponseModel>
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IButtonInteraction _buttonInteraction;
        private readonly IEditContextFactory _editContextFactory;

        public EntitiesInteractionDispatcher(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IRepositoryResolver repositoryResolver,
            IConcurrencyService concurrencyService,
            IButtonInteraction buttonInteraction,
            IEditContextFactory editContextFactory)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _concurrencyService = concurrencyService;
            _buttonInteraction = buttonInteraction;
            _editContextFactory = editContextFactory;
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
            var collection = _collectionResolver.ResolveSetup(request.ListContext.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var (crudType, entityVariant) = await _buttonInteraction.ValidateButtonInteractionAsync(request);

            switch (crudType)
            {
                case CrudType.Create:
                    if (entityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must have an EntityVariant.");
                    }

                    var currentState = pageState.GetCurrentState();

                    if (response is ListViewCommandResponseModel || ShouldFallbackToNavigatingToNodeEditor(collection, currentState))
                    {
                        pageState.PushState(new PageStateModel
                        {
                            PageType = PageType.Node,
                            UsageType = UsageType.New,

                            CollectionAlias = request.ListContext.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = request.ListContext.Parent?.GetParentPath(),
                            Related = request.Related
                        });
                    }
                    else
                    {
                        pageState.PushState(new PageStateModel
                        {
                            PageType = PageType.Collection,
                            UsageType = UsageType.New,


                            CollectionAlias = request.ListContext.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = request.ListContext.Parent?.GetParentPath(),
                            Related = request.Related,

                            // is this the best place here?
                            ActiveTab = currentState?.ActiveTab,
                            CurrentPage = currentState?.CurrentPage ?? 1,
                            MaxPage = currentState?.MaxPage,
                            SearchTerm = currentState?.SearchTerm
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
                        if (!editContext.IsValid())
                        {
                            throw new InvalidEntityException();
                        }

                        await _buttonInteraction.ValidateButtonInteractionAsync(innerRequest);

                        if (editContext.IsModified())
                        {
                            var wrapper = _editContextFactory.GetEditContextWrapper(editContext);
                            await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(wrapper));
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
                        var parentPath = request.ListContext.Parent?.GetParentPath();
                        pageState.ReplaceState(new PageStateModel
                        {
                            PageType = PageType.Collection,
                            UsageType = (collection.ListEditor == null ? UsageType.View : UsageType.Edit)
                             | ((parentPath != null) ? UsageType.NotRoot : UsageType.Root),

                            CollectionAlias = request.ListContext.CollectionAlias,
                            ParentPath = parentPath,
                            Related = request.Related
                        });
                    }
                    break;

                case CrudType.Up:
                    if (pageState.PopState() == null)
                    {
                        var (newParentPath, repositoryAlias, parentId) = ParentPath.RemoveLevel(request.ListContext.Parent?.GetParentPath());

                        if (repositoryAlias == null)
                        {
                            break;
                        }

                        var parentCollection = _collectionResolver.ResolveSetup(repositoryAlias);

                        pageState.ReplaceState(new PageStateModel
                        {
                            PageType = PageType.Node,
                            UsageType = (parentCollection.NodeEditor == null ? UsageType.View : UsageType.Edit)
                             | ((newParentPath != null) ? UsageType.NotRoot : UsageType.Root),

                            CollectionAlias = parentCollection.Alias,
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

        private static bool ShouldFallbackToNavigatingToNodeEditor(ICollectionSetup collection, PageStateModel? currentState)
        {
            return collection.NodeEditor != null;
        }
    }
}
