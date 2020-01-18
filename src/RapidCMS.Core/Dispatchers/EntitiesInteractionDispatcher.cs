using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Dispatchers
{
    internal class EntitiesInteractionDispatcher : 
        IInteractionDispatcher<PersistEntitiesRequestModel, ListViewCommandResponseModel>
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
        
        Task<ListViewCommandResponseModel> IInteractionDispatcher<PersistEntitiesRequestModel, ListViewCommandResponseModel>.InvokeAsync(PersistEntitiesRequestModel request)
        {
            return InvokeAsync(request, new ListViewCommandResponseModel());
        }

        private async Task<T> InvokeAsync<T>(PersistEntitiesRequestModel request, T response)
            where T : ViewCommandResponseModel
        {
            var collection = _collectionResolver.GetCollection(request.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var (crudType, entityVariant) = await _buttonInteraction.ValidateButtonInteractionAsync(request);

            switch (crudType)
            {
                case CrudType.Create:
                    if (entityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must have an EntityVariant.");
                    }
                    if (request.UsageType.HasFlag(UsageType.List))
                    {
                        response.ViewCommand = new NavigateCommand
                        {
                            Uri = UriHelper.Node(
                                Constants.New,
                                request.CollectionAlias,
                                entityVariant,
                                request.ListContext.Parent?.GetParentPath(),
                                null)
                        };
                    }
                    else
                    {
                        response.ViewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = request.CollectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentPath = request.ListContext.Parent?.GetParentPath()?.ToPathString(),
                            Id = null
                        };
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

                    response.ViewCommand = new ReloadCommand(affectedEntities.SelectNotNull(x => x.Id));
                    break;

                case CrudType.None:
                    response.ViewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    response.ViewCommand = new ReloadCommand();
                    break;

                case CrudType.Return:
                    response.ViewCommand = new NavigateCommand
                    {
                        Uri = UriHelper.Collection(
                            Constants.Edit,
                            request.CollectionAlias,
                            request.ListContext.Parent?.GetParentPath())
                    };
                    break;

                case CrudType.Up:
                    var (newParentPath, parentCollectionAlias, parentId) = ParentPath.RemoveLevel(request.ListContext.Parent?.GetParentPath());

                    if (parentCollectionAlias == null)
                    {
                        response.ViewCommand = new NoOperationCommand();
                        break;
                    }

                    var parentCollection = _collectionResolver.GetCollection(parentCollectionAlias);

                    response.ViewCommand = new NavigateCommand
                    {
                        Uri = UriHelper.Node(
                        request.UsageType.HasFlag(UsageType.Edit) ? Constants.Edit : Constants.View,
                        parentCollectionAlias,
                        parentCollection.EntityVariant,
                        newParentPath,
                        parentId)
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
