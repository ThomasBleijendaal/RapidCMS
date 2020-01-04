using System;
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
    }
}
