using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventAggregator.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Authorization;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Messages;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Commands;

namespace RapidCMS.Common.Services
{
    internal class EditContextService : IEditContextService
    {
        private readonly Root _root;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventAggregator _eventAggregator;

        public EditContextService(
            Root root,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService,
            IServiceProvider serviceProvider,
            IEventAggregator eventAggregator)
        {
            _root = root;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
            _serviceProvider = serviceProvider;
            _eventAggregator = eventAggregator;
        }

        public async Task<List<EditContext>> GetEntitiesAsync(UsageType usageType, string collectionAlias, string? parentId, Query query)
        {
            // NOTE: this method does not check authorization

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, parentId);

            var collection = _root.GetCollection(collectionAlias);

            await collection.ProcessDataViewAsync(query, _serviceProvider);

            var existingEntities = await collection.Repository.InternalGetAllAsync(parentId, query);

            return ConvertEditContexts(usageType, collectionAlias, rootEditContext, existingEntities);
        }

        public async Task<List<EditContext>> GetRelatedEntitiesAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, Query query)
        {
            // NOTE: this method does not check authorization

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, null);

            var collection = _root.GetCollection(collectionAlias);

            await collection.ProcessDataViewAsync(query, _serviceProvider);

            var existingEntities = usageType.HasFlag(UsageType.Add)
                ? await collection.Repository.InternalGetAllNonRelatedAsync(relatedEntity, query)
                : await collection.Repository.InternalGetAllRelatedAsync(relatedEntity, query);

            return ConvertEditContexts(usageType, collectionAlias, rootEditContext, existingEntities);
        }

        public async Task<EditContext> GetEntityAsync(UsageType usageType, string collectionAlias, string? variantAlias, string? parentId, string? id)
        {
            var collection = _root.GetCollection(collectionAlias);

            var entity = usageType switch
            {
                UsageType.View => await collection.Repository.InternalGetByIdAsync(id ?? throw new InvalidOperationException($"Cannot View Node when {id} is null"), parentId),
                UsageType.Edit => await collection.Repository.InternalGetByIdAsync(id ?? throw new InvalidOperationException($"Cannot Edit Node when {id} is null"), parentId),
                UsageType.New => await collection.Repository.InternalNewAsync(parentId, collection.GetEntityVariant(variantAlias).Type),
                _ => throw new InvalidOperationException($"UsageType {usageType} is invalid for this method")
            };

            if (entity == null)
            {
                throw new Exception("Failed to get entity for given id(s)");
            }

            await EnsureAuthorizedUserAsync(usageType, entity);

            return new EditContext(entity, usageType | UsageType.Node, _serviceProvider);
        }

        public async Task<EditContext> GetRootAsync(UsageType usageType, string collectionAlias, string? parentId)
        {
            var context = await GetRootEditContextAsync(usageType, collectionAlias, parentId);

            await EnsureAuthorizedUserAsync(usageType, context.Entity);

            return context;
        }

        public async Task<ViewCommand> ProcessEntityActionAsync(UsageType usageType, string collectionAlias, string? parentId, string? id, EditContext editContext, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            var entityVariant = collection.GetEntityVariant(editContext.Entity);

            var nodeEditor = collection.NodeEditor;
            var button = nodeEditor?.FindButton(actionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            await EnsureAuthorizedUserAsync(editContext, button);
            EnsureValidEditContext(editContext, button);

            var relationContainer = editContext.GenerateRelationContainer();

            ViewCommand viewCommand;

            var context = new ButtonContext(parentId, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(editContext, context))
            {
                case CrudType.View:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };
                    break;

                case CrudType.Edit:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };
                    break;

                case CrudType.Update:
                    await collection.Repository.InternalUpdateAsync(id ?? throw new InvalidOperationException(), parentId, editContext.Entity, relationContainer);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new ReloadCommand(id);
                    break;

                case CrudType.Insert:
                    var entity = await collection.Repository.InternalInsertAsync(parentId, editContext.Entity, relationContainer);
                    editContext.SwapEntity(entity);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, editContext.Entity.Id) };
                    break;

                case CrudType.Delete:
                    await collection.Repository.InternalDeleteAsync(id ?? throw new InvalidOperationException(), parentId);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new NavigateCommand { Uri = UriHelper.Collection(Constants.List, collectionAlias, parentId) };
                    break;

                case CrudType.None:
                    viewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Return:
                    viewCommand = new ReturnCommand();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await button.ButtonClickAfterRepositoryActionAsync(editContext, context);

            return viewCommand;
        }

        public async Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, string? parentId, IEnumerable<EditContext> editContexts, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            // TODO: convert to FindButton
            var buttons = usageType.HasFlag(UsageType.List)
                ? collection.ListView?.Buttons
                : collection.ListEditor?.Buttons;
            var button = buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);

            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, parentId);
            var entity = await collection.Repository.InternalNewAsync(parentId, collection.EntityVariant.Type);

            await EnsureAuthorizedUserAsync(rootEditContext, button);

            ViewCommand viewCommand;

            var context = new ButtonContext(parentId, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(rootEditContext, context))
            {
                case CrudType.Create:
                    if (button.EntityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must an {nameof(button.EntityVariant)}.");
                    }
                    if (usageType.HasFlag(UsageType.List))
                    {
                        viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, button.EntityVariant, parentId, null) };
                    }
                    else
                    {
                        viewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = collectionAlias,
                            VariantAlias = button.EntityVariant.Alias,
                            ParentId = parentId,
                            Id = null
                        };
                    }
                    break;

                case CrudType.Update:
                    var contextsToProcess = editContexts.Where(x => x.IsModified()).Where(x => button.RequiresValidForm(x) ? x.IsValid() : true);
                    var affectedEntities = new List<IEntity>();
                    foreach (var editContext in contextsToProcess)
                    {
                        try
                        {
                            await EnsureAuthorizedUserAsync(editContext, button);
                            EnsureValidEditContext(editContext, button);
                            var relationContainer = editContext.GenerateRelationContainer();
                            await collection.Repository.InternalUpdateAsync(editContext.Entity.Id, parentId, editContext.Entity, relationContainer);
                            affectedEntities.Add(editContext.Entity);
                        }
                        catch (Exception)
                        {
                            // do not care about any exception in this case
                        }
                    }
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new ReloadCommand(affectedEntities.Select(x => x.Id));
                    break;

                case CrudType.None:
                    viewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Return:
                    viewCommand = new ReturnCommand();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await button.ButtonClickAfterRepositoryActionAsync(rootEditContext, context);

            return viewCommand;
        }

        public async Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, string? parentId, string id, EditContext editContext, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            // TODO: convert to FindButton
            var buttons = usageType.HasFlag(UsageType.List)
                ? collection.ListView?.Panes?.SelectMany(pane => pane.Buttons)
                : collection.ListEditor?.Panes?.SelectMany(pane => pane.Buttons);
            var button = buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);

            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            await EnsureAuthorizedUserAsync(editContext, button);
            EnsureValidEditContext(editContext, button);

            var relationContainer = editContext.GenerateRelationContainer();

            // since the id is known, get the entity variant from the entity
            var entityVariant = collection.GetEntityVariant(editContext.Entity);

            ViewCommand viewCommand;

            var context = new ButtonContext(parentId, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(editContext, context))
            {
                case CrudType.View:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };
                    break;

                case CrudType.Edit:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };
                    break;

                case CrudType.Update:
                    await collection.Repository.InternalUpdateAsync(id, parentId, editContext.Entity, relationContainer);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new ReloadCommand(id);
                    break;

                case CrudType.Insert:
                    var insertedEntity = await collection.Repository.InternalInsertAsync(parentId, editContext.Entity, relationContainer);
                    editContext.SwapEntity(insertedEntity);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = parentId,
                        Id = editContext.Entity.Id
                    };
                    break;

                case CrudType.Delete:
                    await collection.Repository.InternalDeleteAsync(id, parentId);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.None:
                    viewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Return:
                    viewCommand = new ReturnCommand();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await button.ButtonClickAfterRepositoryActionAsync(editContext, context);

            return viewCommand;
        }

        public async Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, IEnumerable<EditContext> editContexts, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            // TODO: convert to FindButton
            var buttons = usageType.HasFlag(UsageType.List) || usageType.HasFlag(UsageType.Add)
                ? collection.ListView?.Buttons
                : collection.ListEditor?.Buttons;
            var button = buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);

            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, null);
            var newEntity = await collection.Repository.InternalNewAsync(null, collection.EntityVariant.Type);

            await EnsureAuthorizedUserAsync(rootEditContext, button);

            ViewCommand viewCommand;

            var context = new ButtonContext(null, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(rootEditContext, context))
            {
                case CrudType.Create:
                    if (button.EntityVariant == null)
                    {
                        throw new InvalidOperationException();
                    }

                    if (usageType.HasFlag(UsageType.List))
                    {
                        viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, button.EntityVariant, null, null) };
                    }
                    else
                    {
                        viewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = collectionAlias,
                            VariantAlias = button.EntityVariant.Alias,
                            ParentId = null,
                            Id = null
                        };
                    }
                    break;

                case CrudType.Update:
                    var contextsToProcess = editContexts.Where(x => x.IsModified()).Where(x => button.RequiresValidForm(x) ? x.IsValid() : true);
                    var affectedEntities = new List<IEntity>();
                    foreach (var editContext in contextsToProcess)
                    {
                        try
                        {
                            var updatedEntity = editContext.Entity;
                            await EnsureAuthorizedUserAsync(editContext, button);
                            EnsureValidEditContext(editContext, button);
                            var relationContainer = editContext.GenerateRelationContainer();
                            await collection.Repository.InternalUpdateAsync(updatedEntity.Id, null, updatedEntity, relationContainer);
                            affectedEntities.Add(updatedEntity);
                        }
                        catch (Exception)
                        {
                            // do not care about exceptions here
                        }
                    }
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new ReloadCommand(affectedEntities.Select(x => x.Id));
                    break;

                case CrudType.Add:
                    viewCommand = new UpdateParameterCommand
                    {
                        Action = Constants.Add,
                        CollectionAlias = collectionAlias,
                        VariantAlias = null,
                        ParentId = null,
                        Id = null
                    };
                    break;

                case CrudType.None:
                    viewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Return:
                    viewCommand = new ReturnCommand();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await button.ButtonClickAfterRepositoryActionAsync(rootEditContext, context);

            return viewCommand;
        }

        public async Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, string id, EditContext editContext, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            // TODO: convert to FindButton
            var buttons = usageType.HasFlag(UsageType.List) || usageType.HasFlag(UsageType.Add)
                ? collection.ListView?.Panes?.SelectMany(pane => pane.Buttons)
                : collection.ListEditor?.Panes?.SelectMany(pane => pane.Buttons);
            var button = buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);

            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            await EnsureAuthorizedUserAsync(editContext, button);

            EnsureValidEditContext(editContext, button);

            var relationContainer = editContext.GenerateRelationContainer();

            // since the id is known, get the entity variant from the entity
            var entityVariant = collection.GetEntityVariant(editContext.Entity);

            ViewCommand viewCommand;

            var context = new ButtonContext(null, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(editContext, context))
            {
                case CrudType.View:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, null, id) };
                    break;

                case CrudType.Edit:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, null, id) };
                    break;

                case CrudType.Update:
                    await collection.Repository.InternalUpdateAsync(id, null, editContext.Entity, relationContainer);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new ReloadCommand(id);
                    break;

                case CrudType.Insert:
                    var insertedEntity = await collection.Repository.InternalInsertAsync(null, editContext.Entity, relationContainer);
                    editContext.SwapEntity(insertedEntity);
                    await collection.Repository.InternalAddAsync(relatedEntity, editContext.Entity.Id);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    viewCommand = new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = null,
                        Id = editContext.Entity.Id
                    };
                    break;

                case CrudType.Delete:

                    await collection.Repository.InternalDeleteAsync(id, null);
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Pick:

                    await collection.Repository.InternalAddAsync(relatedEntity, id);
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Remove:

                    await collection.Repository.InternalRemoveAsync(relatedEntity, id);
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.None:
                    viewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    viewCommand = new ReloadCommand();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await button.ButtonClickAfterRepositoryActionAsync(editContext, context);

            return viewCommand;
        }

        private List<EditContext> ConvertEditContexts(UsageType usageType, string collectionAlias, EditContext rootEditContext, IEnumerable<IEntity> existingEntities)
        {
            if (usageType == UsageType.List)
            {
                return existingEntities
                    .Select(ent => new EditContext(ent, UsageType.Node | UsageType.Edit, _serviceProvider))
                    .ToList();
            }
            else if (usageType.HasFlag(UsageType.Add))
            {
                return existingEntities
                    .Select(ent => new EditContext(ent, UsageType.Node | UsageType.Pick, _serviceProvider))
                    .ToList();
            }
            else if (usageType.HasFlag(UsageType.Edit) || usageType.HasFlag(UsageType.New))
            {
                var entities = existingEntities
                    .Select(ent => new EditContext(ent, UsageType.Node | UsageType.Edit, _serviceProvider))
                    .ToList();

                if (usageType.HasFlag(UsageType.New))
                {
                    entities.Insert(0, new EditContext(rootEditContext.Entity, UsageType.Node | UsageType.New, _serviceProvider));
                }

                return entities;
            }
            else
            {
                throw new NotImplementedException($"Failed to process {usageType} for collection {collectionAlias}");
            }
        }

        private async Task<EditContext> GetRootEditContextAsync(UsageType usageType, string collectionAlias, string? parentId)
        {
            var collection = _root.GetCollection(collectionAlias);
            var newEntity = await collection.Repository.InternalNewAsync(parentId, collection.EntityVariant.Type);
            return new EditContext(newEntity, usageType | UsageType.List, _serviceProvider);
        }

        private static void EnsureValidEditContext(EditContext editContext, Button button)
        {
            if (button.RequiresValidForm(editContext) && !editContext.IsValid())
            {
                throw new InvalidEntityException();
            }
        }

        private Task EnsureAuthorizedUserAsync(EditContext editContext, Button button)
        {
            return EnsureAuthorizedUserAsync(button.GetOperation(editContext), editContext.Entity);
        }

        private Task EnsureAuthorizedUserAsync(UsageType usageType, IEntity entity)
        {
            return EnsureAuthorizedUserAsync(Operations.GetOperationForUsageType(usageType), entity);
        }

        private async Task EnsureAuthorizedUserAsync(OperationAuthorizationRequirement operation, IEntity entity)
        {
            var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                entity,
                operation);

            if (!authorizationChallenge.Succeeded)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
