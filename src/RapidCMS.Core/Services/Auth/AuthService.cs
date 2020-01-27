using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Services.Auth
{
    internal class AuthService : IAuthService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IButtonActionHandlerResolver _buttonActionHandlerResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            IAuthorizationService authorizationService,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _buttonActionHandlerResolver = buttonActionHandlerResolver;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<bool> IsUserAuthorizedAsync(UsageType usageType, IEntity entity)
        {
            return IsUserAuthorizedAsync(Operations.GetOperationForUsageType(usageType), entity);
        }

        public async Task EnsureAuthorizedUserAsync(UsageType usageType, IEntity entity)
        {
            if (!await IsUserAuthorizedAsync(usageType, entity))
            {
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<bool> IsUserAuthorizedAsync(OperationAuthorizationRequirement operation, IEntity entity)
        {
            var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                entity,
                operation);

            return authorizationChallenge.Succeeded;
        }

        public async Task EnsureAuthorizedUserAsync(OperationAuthorizationRequirement operation, IEntity entity)
        {
            if (!await IsUserAuthorizedAsync(operation, entity))
            {
                throw new UnauthorizedAccessException();
            }
        }

        public Task<bool> IsUserAuthorizedAsync(EditContext editContext, IButtonSetup button)
        {
            var handler = _buttonActionHandlerResolver.GetButtonActionHandler(button);

            return IsUserAuthorizedAsync(handler.GetOperation(button, editContext), editContext.Entity);
        }

        public async Task EnsureAuthorizedUserAsync(EditContext editContext, IButtonSetup button)
        {
            if (!await IsUserAuthorizedAsync(editContext, button))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
