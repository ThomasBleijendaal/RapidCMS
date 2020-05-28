using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;
using RapidCMS.Core.Repositories;

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

        public EntityInteractionDispatcher(
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
            var repositoryContext = new RepositoryContext(request.EditContext.CollectionAlias);

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

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(repositoryContext, updateContext));

                    if (request.EditContext.IsReordered())
                    {
                        await _concurrencyService.EnsureCorrectConcurrencyAsync(
                            () => repository.ReorderAsync(repositoryContext, request.EditContext.ReorderedBeforeId, request.EditContext.Entity.Id!, request.EditContext.Parent));
                    }

                    response.RefreshIds = new[] { request.EditContext.Entity.Id! };
                    break;

                case CrudType.Insert:
                    var insertContext = _editContextFactory.GetEditContextWrapper(request.EditContext);
                    if (!insertContext.IsValid())
                    {
                        throw new InvalidEntityException();
                    }

                    var newEntity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.InsertAsync(repositoryContext, insertContext));
                    if (newEntity == null)
                    {
                        throw new Exception("Inserting the new entity failed.");
                    }

                    if (request is PersistRelatedEntityRequestModel related)
                    {
                        await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(repositoryContext, related.Related, request.EditContext.Entity.Id!));
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
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.DeleteAsync(repositoryContext, request.EditContext.Entity.Id!, request.EditContext.Parent));

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

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(repositoryContext, relationRequest.Related, request.EditContext.Entity.Id!));

                    break;

                case CrudType.Remove when request is PersistRelatedEntityRequestModel relationRequest:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.RemoveAsync(repositoryContext, relationRequest.Related, request.EditContext.Entity.Id!));

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
