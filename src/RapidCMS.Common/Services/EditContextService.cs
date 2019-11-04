using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Commands;
using RapidCMS.Common.Providers;

namespace RapidCMS.Common.Services
{
    internal class EditContextService : IEditContextService
    {
        private readonly ICollectionProvider _collectionProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IParentService _parentService;
        private readonly SemaphoreSlim _semaphore;

        public EditContextService(
            ICollectionProvider collectionProvider,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService,
            IServiceProvider serviceProvider,
            IParentService parentService,
            SemaphoreSlim semaphore)
        {
            _collectionProvider = collectionProvider;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
            _serviceProvider = serviceProvider;
            _parentService = parentService;
            _semaphore = semaphore;
        }

        public async Task<List<EditContext>> GetEntitiesAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, Query query)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);

            var dummyEntity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalNewAsync(null, collection.EntityVariant.Type));
            await EnsureAuthorizedUserAsync(usageType, dummyEntity);

            await collection.ProcessDataViewAsync(query, _serviceProvider);

            var parent = await _parentService.GetParentAsync(parentPath);

            var existingEntities = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalGetAllAsync(parent, query));

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, parent);

            return ConvertEditContexts(usageType, collectionAlias, rootEditContext, existingEntities, parent);
        }

        public async Task<List<EditContext>> GetRelatedEntitiesAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, Query query)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);

            var dummyEntity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalNewAsync(null, collection.EntityVariant.Type));
            await EnsureAuthorizedUserAsync(usageType, dummyEntity);

            await collection.ProcessDataViewAsync(query, _serviceProvider);

            var existingEntities = usageType.HasFlag(UsageType.Add)
                ? await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalGetAllNonRelatedAsync(relatedEntity, query))
                : await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalGetAllRelatedAsync(relatedEntity, query));

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, null);

            return ConvertEditContexts(usageType, collectionAlias, rootEditContext, existingEntities, default);
        }

        public async Task<EditContext> GetEntityAsync(UsageType usageType, string collectionAlias, string? variantAlias, ParentPath? parentPath, string? id)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);

            var parent = await _parentService.GetParentAsync(parentPath);

            var entity = (usageType & ~UsageType.Node) switch
            {
                UsageType.View => await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalGetByIdAsync(id ?? throw new InvalidOperationException($"Cannot View Node when {id} is null"), parent)),
                UsageType.Edit => await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalGetByIdAsync(id ?? throw new InvalidOperationException($"Cannot Edit Node when {id} is null"), parent)),
                UsageType.New => await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalNewAsync(parent, collection.GetEntityVariant(variantAlias).Type)),
                _ => throw new InvalidOperationException($"UsageType {usageType} is invalid for this method")
            };

            if (entity == null)
            {
                throw new Exception("Failed to get entity for given id(s)");
            }

            await EnsureAuthorizedUserAsync(usageType, entity);

            return new EditContext(collectionAlias, entity, parent, usageType | UsageType.Node, _serviceProvider);
        }

        public async Task<EditContext> GetRootAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath)
        {
            var parent = await _parentService.GetParentAsync(parentPath);

            var context = await GetRootEditContextAsync(usageType, collectionAlias, parent);

            await EnsureAuthorizedUserAsync(usageType, context.Entity);

            return context;
        }

        public async Task<ViewCommand> ProcessEntityActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, string? id, EditContext editContext, string actionId, object? customData)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);

            var entityVariant = collection.GetEntityVariant(editContext.Entity);

            var button = collection.FindButton(actionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            await EnsureAuthorizedUserAsync(editContext, button);
            EnsureValidEditContext(editContext, button);

            var relationContainer = editContext.GenerateRelationContainer();

            var parent = await _parentService.GetParentAsync(parentPath);

            ViewCommand viewCommand;

            var context = new ButtonContext(parent, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(editContext, context))
            {
                case CrudType.View:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentPath, id) };
                    break;

                case CrudType.Edit:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentPath, id) };
                    break;

                case CrudType.Update:
                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalUpdateAsync(id ?? throw new InvalidOperationException(), parent, editContext.Entity, relationContainer));
                    viewCommand = new ReloadCommand(id);
                    break;

                case CrudType.Insert:
                    var entity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalInsertAsync(parent, editContext.Entity, relationContainer));
                    if (entity == null)
                    {
                        throw new Exception("Inserting the new entity failed.");
                    }
                    editContext.SwapEntity(entity);
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentPath, editContext.Entity.Id) };
                    break;

                case CrudType.Delete:
                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalDeleteAsync(id ?? throw new InvalidOperationException(), parent));
                    viewCommand = new NavigateCommand { Uri = UriHelper.Collection(Constants.List, collectionAlias, parentPath) };
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

        public async Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, IEnumerable<EditContext> editContexts, string actionId, object? customData)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);

            var button = collection.FindButton(actionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var parent = await _parentService.GetParentAsync(parentPath);

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, parent);
            var entity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalNewAsync(parent, collection.EntityVariant.Type));

            await EnsureAuthorizedUserAsync(rootEditContext, button);

            ViewCommand viewCommand;

            var context = new ButtonContext(parent, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(rootEditContext, context))
            {
                case CrudType.Create:
                    if (button.EntityVariant == null)
                    {
                        throw new InvalidOperationException($"Button of type {CrudType.Create} must an {nameof(button.EntityVariant)}.");
                    }
                    if (usageType.HasFlag(UsageType.List))
                    {
                        viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, button.EntityVariant, parentPath, null) };
                    }
                    else
                    {
                        viewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = collectionAlias,
                            VariantAlias = button.EntityVariant.Alias,
                            ParentPath = parentPath?.ToPathString(),
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
                            await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalUpdateAsync(editContext.Entity.Id, parent, editContext.Entity, relationContainer));
                            affectedEntities.Add(editContext.Entity);
                        }
                        catch (Exception)
                        {
                            // do not care about any exception in this case
                        }
                    }
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

        public async Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, string id, EditContext editContext, string actionId, object? customData)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);

            var button = collection.FindButton(actionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            await EnsureAuthorizedUserAsync(editContext, button);
            EnsureValidEditContext(editContext, button);

            var relationContainer = editContext.GenerateRelationContainer();

            // since the id is known, get the entity variant from the entity
            var entityVariant = collection.GetEntityVariant(editContext.Entity);

            var parent = await _parentService.GetParentAsync(parentPath);

            ViewCommand viewCommand;

            var context = new ButtonContext(parent, customData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(editContext, context))
            {
                case CrudType.View:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentPath, id) };
                    break;

                case CrudType.Edit:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentPath, id) };
                    break;

                case CrudType.Update:
                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalUpdateAsync(id, parent, editContext.Entity, relationContainer));
                    viewCommand = new ReloadCommand(id);
                    break;

                case CrudType.Insert:
                    var insertedEntity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalInsertAsync(parent, editContext.Entity, relationContainer));
                    if (insertedEntity == null)
                    {
                        throw new Exception("Inserting the new entity failed.");
                    }
                    editContext.SwapEntity(insertedEntity);
                    viewCommand = new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentPath = parentPath?.ToPathString(),
                        Id = editContext.Entity.Id
                    };
                    break;

                case CrudType.Delete:
                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalDeleteAsync(id, parent));
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
            var collection = _collectionProvider.GetCollection(collectionAlias);

            var button = collection.FindButton(actionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            var rootEditContext = await GetRootEditContextAsync(usageType, collectionAlias, null);
            var newEntity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalNewAsync(null, collection.EntityVariant.Type));

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
                        viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, button.EntityVariant, default, null) };
                    }
                    else
                    {
                        viewCommand = new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = collectionAlias,
                            VariantAlias = button.EntityVariant.Alias,
                            ParentPath = null,
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
                            await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalUpdateAsync(updatedEntity.Id, null, updatedEntity, relationContainer));
                            affectedEntities.Add(updatedEntity);
                        }
                        catch (Exception)
                        {
                            // do not care about exceptions here
                        }
                    }
                    viewCommand = new ReloadCommand(affectedEntities.Select(x => x.Id));
                    break;

                case CrudType.Add:
                    viewCommand = new UpdateParameterCommand
                    {
                        Action = Constants.Add,
                        CollectionAlias = collectionAlias,
                        VariantAlias = null,
                        ParentPath = null,
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
            var collection = _collectionProvider.GetCollection(collectionAlias);

            var button = collection.FindButton(actionId);
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
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, default, id) };
                    break;

                case CrudType.Edit:
                    viewCommand = new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, default, id) };
                    break;

                case CrudType.Update:
                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalUpdateAsync(id, null, editContext.Entity, relationContainer));
                    viewCommand = new ReloadCommand(id);
                    break;

                case CrudType.Insert:
                    var insertedEntity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalInsertAsync(null, editContext.Entity, relationContainer));
                    if (insertedEntity == null)
                    {
                        throw new Exception("Inserting the new entity failed.");
                    }
                    editContext.SwapEntity(insertedEntity);
                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalAddAsync(relatedEntity, editContext.Entity.Id));
                    viewCommand = new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentPath = null,
                        Id = editContext.Entity.Id
                    };
                    break;

                case CrudType.Delete:

                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalDeleteAsync(id, null));
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Pick:

                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalAddAsync(relatedEntity, id));
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Remove:

                    await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalRemoveAsync(relatedEntity, id));
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

        private List<EditContext> ConvertEditContexts(UsageType usageType, string collectionAlias, EditContext rootEditContext, IEnumerable<IEntity> existingEntities, IParent? parent)
        {
            if (usageType == UsageType.List)
            {
                return existingEntities
                    .Select(ent => new EditContext(collectionAlias, ent, parent, UsageType.Node | UsageType.Edit, _serviceProvider))
                    .ToList();
            }
            else if (usageType.HasFlag(UsageType.Add))
            {
                return existingEntities
                    .Select(ent => new EditContext(collectionAlias, ent, parent, UsageType.Node | UsageType.Pick, _serviceProvider))
                    .ToList();
            }
            else if (usageType.HasFlag(UsageType.Edit) || usageType.HasFlag(UsageType.New))
            {
                var entities = existingEntities
                    .Select(ent => new EditContext(collectionAlias, ent, parent, UsageType.Node | UsageType.Edit, _serviceProvider))
                    .ToList();

                if (usageType.HasFlag(UsageType.New))
                {
                    entities.Insert(0, new EditContext(collectionAlias, rootEditContext.Entity, parent, UsageType.Node | UsageType.New, _serviceProvider));
                }

                return entities;
            }
            else
            {
                throw new NotImplementedException($"Failed to process {usageType} for collection {collectionAlias}");
            }
        }

        private async Task<EditContext> GetRootEditContextAsync(UsageType usageType, string collectionAlias, IParent? parent)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);
            var newEntity = await EnsureCorrectConcurrencyAsync(() => collection.Repository.InternalNewAsync(parent, collection.EntityVariant.Type));

            await EnsureAuthorizedUserAsync(usageType, newEntity);

            return new EditContext(collectionAlias, newEntity, parent, usageType | UsageType.List, _serviceProvider);
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

        private async Task EnsureCorrectConcurrencyAsync(Func<Task> function)
        {
            await _semaphore.WaitAsync();

            try
            {
                await function.Invoke();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<T> EnsureCorrectConcurrencyAsync<T>(Func<Task<T>> function)
        {
            await _semaphore.WaitAsync();

            try
            {
                return await function.Invoke();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
