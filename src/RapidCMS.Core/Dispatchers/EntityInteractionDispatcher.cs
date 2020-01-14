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
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Dispatchers
{
    internal class EntityInteractionDispatcher :
        IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel>,
        IInteractionDispatcher<PersistEntityRequestModel, NodeInListViewCommandResponseModel>
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
                    response.ViewCommand = new NavigateCommand
                    {
                        Uri = NodeUri(Constants.View, request, entityVariant)
                    };
                    break;

                case CrudType.Edit:
                    response.ViewCommand = new NavigateCommand
                    {
                        Uri = NodeUri(Constants.Edit, request, entityVariant)
                    };
                    break;

                case CrudType.Update:
                    await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(request.EditContext));
                    response.ViewCommand = new ReloadCommand(request.EditContext.Entity.Id!);
                    break;

                case CrudType.Insert:
                    var newEntity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.InsertAsync(request.EditContext));
                    if (newEntity == null)
                    {
                        throw new Exception("Inserting the new entity failed.");
                    }
                    request.EditContext.SwapEntity(newEntity);

                    // TODO: add to related collection

                    if (response is NodeViewCommandResponseModel)
                    {
                        response.ViewCommand = new NavigateCommand
                        {
                            Uri = NodeUri(Constants.Edit, request, entityVariant)
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
                            Uri = CollectionUri(Constants.View, request)
                        };
                    }
                    else if (response is NodeInListViewCommandResponseModel)
                    {
                        response.ViewCommand = new ReloadCommand();
                    }

                    break;

                case CrudType.Pick:
                    response.ViewCommand = default!;
                    // TODO: add to related collection
                    break;

                case CrudType.Remove:
                    response.ViewCommand = default!;
                    // TODO: add to related collection
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
                        Uri = CollectionUri(collection.ListEditor == null ? Constants.View : Constants.Edit, request)
                    };
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await _buttonInteraction.CompleteButtonInteractionAsync(request);

            return response;
        }

        private static string CollectionUri(string action, PersistEntityRequestModel request)
        {
            return UriHelper.Collection(action, request.EditContext.CollectionAlias, request.EditContext.Parent?.GetParentPath());
        }

        private static string NodeUri(string action, PersistEntityRequestModel request, EntityVariantSetup entityVariant)
        {
            return UriHelper.Node(action, request.EditContext.CollectionAlias, entityVariant, request.EditContext.Parent?.GetParentPath(), request.EditContext.Entity.Id);
        }
    }
}
