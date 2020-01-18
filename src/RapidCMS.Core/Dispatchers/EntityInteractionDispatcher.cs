using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

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
            var relationRequest = request as PersistRelatedEntityRequestModel;

            var collection = _collectionResolver.GetCollection(request.EditContext.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var entityVariant = collection.GetEntityVariant(request.EditContext.Entity);

            var crudType = await _buttonInteraction.ValidateButtonInteractionAsync(request);

            switch (crudType)
            {
                case CrudType.View:
                    response.ViewCommand = new NavigateCommand
                    {
                        Uri = UriHelper.Node(Constants.View, request.EditContext.CollectionAlias, entityVariant, request.EditContext.Parent?.GetParentPath(), request.EditContext.Entity.Id)
                    };
                    break;

                case CrudType.Edit:
                    response.ViewCommand = new NavigateCommand
                    {
                        Uri = UriHelper.Node(Constants.Edit, request.EditContext.CollectionAlias, entityVariant, request.EditContext.Parent?.GetParentPath(), request.EditContext.Entity.Id)
                    };
                    break;

                case CrudType.Update:
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(request.EditContext));

                    if (request.EditContext.IsReordered())
                    {
                        await _concurrencyService.EnsureCorrectConcurrencyAsync(
                            () => repository.ReorderAsync(request.EditContext.ReorderedBeforeId, request.EditContext.Entity.Id!, request.EditContext.Parent));
                    }

                    response.ViewCommand = new ReloadCommand(request.EditContext.Entity.Id!);
                    break;

                case CrudType.Insert:
                    var newEntity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.InsertAsync(request.EditContext));
                    if (newEntity == null)
                    {
                        throw new Exception("Inserting the new entity failed.");
                    }

                    if (response is NodeViewCommandResponseModel)
                    {
                        response.ViewCommand = new NavigateCommand
                        {
                            Uri = UriHelper.Node(Constants.Edit, request.EditContext.CollectionAlias, entityVariant, request.EditContext.Parent?.GetParentPath(), newEntity.Id)
                        };
                    }
                    else if (response is NodeInListViewCommandResponseModel)
                    {
                        response.ViewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = request.EditContext.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = request.EditContext.Parent?.GetParentPath()?.ToPathString(),
                            Id = newEntity.Id
                        };
                    }

                    break;

                case CrudType.Delete:
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.DeleteAsync(request.EditContext.Entity.Id!, request.EditContext.Parent));

                    if (response is NodeViewCommandResponseModel)
                    {
                        response.ViewCommand = new NavigateCommand
                        {
                            Uri = UriHelper.Collection(collection.ListEditor == null ? Constants.List : Constants.Edit, request.EditContext.CollectionAlias, request.EditContext.Parent?.GetParentPath())
                        };
                    }
                    else if (response is NodeInListViewCommandResponseModel)
                    {
                        response.ViewCommand = new ReloadCommand();
                    }

                    break;

                case CrudType.Pick when relationRequest != null:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.AddAsync(relationRequest.Related, request.EditContext.Entity.Id!));
                    response.ViewCommand = new ReloadCommand();

                    break;

                case CrudType.Remove when relationRequest != null:

                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.RemoveAsync(relationRequest.Related, request.EditContext.Entity.Id!));
                    response.ViewCommand = new ReloadCommand();

                    break;

                case CrudType.None:
                    response.ViewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    response.ViewCommand = new ReloadCommand();
                    break;

                case CrudType.Up:
                    response.ViewCommand = new NavigateCommand
                    {
                        Uri = UriHelper.Collection(collection.ListEditor == null ? Constants.List : Constants.Edit, request.EditContext.CollectionAlias, request.EditContext.Parent?.GetParentPath())
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
