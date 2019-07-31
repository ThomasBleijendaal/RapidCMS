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
            // TODO: this method does not check authorization

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, parentId);

            var collection = _root.GetCollection(collectionAlias);

            await collection.ProcessDataViewAsync(query, _serviceProvider);

            var existingEntities = await collection.Repository.InternalGetAllAsync(parentId, query);

            return ConvertEditContexts(usageType, collectionAlias, rootEditContext, existingEntities);
        }

        public async Task<List<EditContext>> GetRelatedEntitiesAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, Query query)
        {
            // TODO: this method does not check authorization

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, null);

            var collection = _root.GetCollection(collectionAlias);

            await collection.ProcessDataViewAsync(query, _serviceProvider);

            var existingEntities = usageType.HasFlag(UsageType.Add)
                ? await collection.Repository.InternalGetAllNonRelatedAsync(relatedEntity, query)
                : await collection.Repository.InternalGetAllRelatedAsync(relatedEntity, query);

            return ConvertEditContexts(usageType, collectionAlias, rootEditContext, existingEntities);
        }

        public async Task<EditContext> GetEntityAsync(UsageType usageType, string collectionAlias, string variantAlias, string? parentId, string? id)
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
            var button = nodeEditor?.Buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var buttonCrudType = button.GetCrudType();

            var updatedEntity = editContext.Entity;

            await EnsureAuthorizedUserAsync(buttonCrudType, updatedEntity);
            EnsureValidEditContext(editContext, button);

            var relationContainer = editContext.DataContext.GenerateRelationContainer();

            if (button is CustomButton customButton)
            {
                buttonCrudType = await customButton.HandleActionAsync(parentId, id, customData) ?? buttonCrudType;
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Edit:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Update:
                    await collection.Repository.InternalUpdateAsync(id ?? throw new InvalidOperationException(), parentId, updatedEntity, relationContainer);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    return new ReloadCommand();

                case CrudType.Insert:
                    var entity = await collection.Repository.InternalInsertAsync(parentId, updatedEntity, relationContainer);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, entity.Id) };

                case CrudType.Delete:
                    await collection.Repository.InternalDeleteAsync(id ?? throw new InvalidOperationException(), parentId);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    return new NavigateCommand { Uri = UriHelper.Collection(Constants.List, collectionAlias, parentId) };

                case CrudType.None:
                    return new NoOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                case CrudType.Return:
                    return new ReturnCommand();

                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, string? parentId, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            var buttons = usageType.HasFlag(UsageType.List)
                ? collection.ListView?.Buttons
                : collection.ListEditor?.Buttons;
            var button = buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);

            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var entity = await collection.Repository.InternalNewAsync(parentId, collection.EntityVariant.Type);

            var buttonCrudType = button.GetCrudType();

            await EnsureAuthorizedUserAsync(buttonCrudType, entity);

            if (button is CustomButton customButton)
            {
                buttonCrudType = await customButton.HandleActionAsync(parentId, null, customData) ?? buttonCrudType;
            }

            switch (buttonCrudType)
            {
                case CrudType.Create:
                    if (!(button.Metadata is EntityVariant entityVariant))
                    {
                        throw new InvalidOperationException();
                    }

                    if (usageType.HasFlag(UsageType.List))
                    {
                        return new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, entityVariant, parentId, null) };
                    }
                    else
                    {
                        return new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = collectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentId = parentId,
                            Id = null
                        };
                    }

                case CrudType.None:
                    return new NoOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                case CrudType.Return:
                    return new ReturnCommand();

                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, string? parentId, string id, EditContext editContext, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            var buttons = usageType.HasFlag(UsageType.List)
                ? collection.ListView?.ViewPanes?.SelectMany(pane => pane.Buttons)
                : collection.ListEditor?.EditorPanes?.SelectMany(pane => pane.Buttons);
            var button = buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);

            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var buttonCrudType = button.GetCrudType();

            var updatedEntity = editContext.Entity;

            await EnsureAuthorizedUserAsync(buttonCrudType, updatedEntity);
            EnsureValidEditContext(editContext, button);

            var relationContainer = editContext.DataContext.GenerateRelationContainer();

            // since the id is known, get the entity variant from the entity
            var entityVariant = collection.GetEntityVariant(updatedEntity);

            if (button is CustomButton customButton)
            {
                buttonCrudType = await customButton.HandleActionAsync(parentId, id, customData) ?? buttonCrudType;
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Edit:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Update:
                    await collection.Repository.InternalUpdateAsync(id, parentId, updatedEntity, relationContainer);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    return new ReloadCommand();

                case CrudType.Insert:
                    updatedEntity = await collection.Repository.InternalInsertAsync(parentId, updatedEntity, relationContainer);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    return new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = parentId,
                        Id = updatedEntity.Id
                    };

                case CrudType.Delete:

                    await collection.Repository.InternalDeleteAsync(id, parentId);
                    await _eventAggregator.PublishAsync(new CollectionUpdatedMessage(collectionAlias));
                    return new ReloadCommand();

                case CrudType.None:
                    return new NoOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                case CrudType.Return:
                    return new ReturnCommand();

                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            var buttons = usageType.HasFlag(UsageType.List) || usageType.HasFlag(UsageType.Add)
                ? collection.ListView?.Buttons
                : collection.ListEditor?.Buttons;
            var button = buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);

            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var newEntity = await collection.Repository.InternalNewAsync(null, collection.EntityVariant.Type);

            var buttonCrudType = button.GetCrudType();

            await EnsureAuthorizedUserAsync(buttonCrudType, newEntity);

            if (button is CustomButton customButton)
            {
                buttonCrudType = await customButton.HandleActionAsync(null, null, customData) ?? buttonCrudType;
            }

            switch (buttonCrudType)
            {
                case CrudType.Create:
                    if (!(button.Metadata is EntityVariant entityVariant))
                    {
                        throw new InvalidOperationException();
                    }

                    if (usageType.HasFlag(UsageType.List))
                    {
                        return new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, entityVariant, null, null) };
                    }
                    else
                    {
                        return new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = collectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentId = null,
                            Id = null
                        };
                    }

                case CrudType.Add:
                    return new UpdateParameterCommand
                    {
                        Action = Constants.Add,
                        CollectionAlias = collectionAlias,
                        VariantAlias = null,
                        ParentId = null,
                        Id = null
                    };

                case CrudType.None:
                    return new NoOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                case CrudType.Return:
                    return new ReturnCommand();

                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, string id, EditContext editContext, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            var buttons = usageType.HasFlag(UsageType.List) || usageType.HasFlag(UsageType.Add)
                ? collection.ListView?.ViewPanes?.SelectMany(pane => pane.Buttons)
                : collection.ListEditor?.EditorPanes?.SelectMany(pane => pane.Buttons);
            var button = buttons?.GetAllButtons().FirstOrDefault(x => x.ButtonId == actionId);

            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var buttonCrudType = button.GetCrudType();

            var updatedEntity = editContext.Entity;

            await EnsureAuthorizedUserAsync(buttonCrudType, updatedEntity);

            EnsureValidEditContext(editContext, button);

            var relationContainer = editContext.DataContext.GenerateRelationContainer();

            // since the id is known, get the entity variant from the entity
            var entityVariant = collection.GetEntityVariant(updatedEntity);

            if (button is CustomButton customButton)
            {
                buttonCrudType = await customButton.HandleActionAsync(null, id, customData) ?? buttonCrudType;
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, null, id) };

                case CrudType.Edit:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, null, id) };

                case CrudType.Update:
                    await collection.Repository.InternalUpdateAsync(id, null, updatedEntity, relationContainer);
                    return new ReloadCommand();

                case CrudType.Insert:
                    updatedEntity = await collection.Repository.InternalInsertAsync(null, updatedEntity, relationContainer);
                    await collection.Repository.InternalAddAsync(relatedEntity, updatedEntity.Id);
                    return new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = null,
                        Id = updatedEntity.Id
                    };

                case CrudType.Delete:

                    await collection.Repository.InternalDeleteAsync(id, null);
                    return new ReloadCommand();

                case CrudType.Pick:

                    await collection.Repository.InternalAddAsync(relatedEntity, id);
                    return new ReloadCommand();

                case CrudType.Remove:

                    await collection.Repository.InternalRemoveAsync(relatedEntity, id);
                    return new ReloadCommand();

                case CrudType.None:
                    return new NoOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                default:
                    throw new InvalidOperationException();
            }
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
            if (button.RequiresValidForm && !editContext.IsValid())
            {
                throw new InvalidEntityException();
            }
        }

        private Task EnsureAuthorizedUserAsync(CrudType crudType, IEntity entity)
        {
            return EnsureAuthorizedUserAsync(Operations.GetOperationForCrudType(crudType), entity);
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
