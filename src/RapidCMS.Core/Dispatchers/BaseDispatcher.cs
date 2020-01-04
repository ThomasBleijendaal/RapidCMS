using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Dispatchers
{
    internal class BaseDispatcher
    {
        protected readonly ICollectionResolver _collectionResolver;
        protected readonly IRepositoryResolver _repositoryResolver;
        protected readonly IParentService _parentService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IServiceProvider _serviceProvider;
        private readonly SemaphoreSlim _semaphore;

        public BaseDispatcher(
            ICollectionResolver collectionResolver,
            IRepositoryResolver repositoryResolver,
            IParentService parentService,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            SemaphoreSlim semaphore)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _parentService = parentService;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
            _semaphore = semaphore;
        }

        protected void EnsureValidEditContext(EditContext editContext, ButtonSetup button)
        {
            if (button.RequiresValidForm(editContext) && !editContext.IsValid())
            {
                throw new InvalidEntityException();
            }
        }

        protected Task EnsureAuthorizedUserAsync(EditContext editContext, ButtonSetup button)
        {
            return EnsureAuthorizedUserAsync(button.GetOperation(editContext), editContext.Entity);
        }

        protected Task EnsureAuthorizedUserAsync(UsageType usageType, IEntity entity)
        {
            return EnsureAuthorizedUserAsync(Operations.GetOperationForUsageType(usageType), entity);
        }

        protected async Task EnsureAuthorizedUserAsync(OperationAuthorizationRequirement operation, IEntity entity)
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

        protected async Task EnsureCorrectConcurrencyAsync(Func<Task> function)
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

        protected async Task<T> EnsureCorrectConcurrencyAsync<T>(Func<Task<T>> function)
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

        private List<EditContext> ConvertEditContexts(UsageType usageType, string collectionAlias, EditContext rootEditContext, IEnumerable<IEntity> existingEntities, IParent? parent)
        {
            if ((usageType & ~(UsageType.Root | UsageType.NotRoot)) == UsageType.List)
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
    }
}
