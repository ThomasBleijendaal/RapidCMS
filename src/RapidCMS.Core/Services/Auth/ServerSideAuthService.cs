using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Services.Auth
{
    internal class ServerSideAuthService : IAuthService
    {
        private readonly IButtonActionHandlerResolver _buttonActionHandlerResolver;
        private readonly IAuthorizationService _authorizationService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public ServerSideAuthService(
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            IAuthorizationService authorizationService,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _buttonActionHandlerResolver = buttonActionHandlerResolver;
            _authorizationService = authorizationService;
            _authenticationStateProvider = authenticationStateProvider;
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
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null)
            {
                return false;
            }

            var authorizationChallenge = await _authorizationService.AuthorizeAsync(user, entity, operation);

            return authorizationChallenge.Succeeded;
        }

        public async Task EnsureAuthorizedUserAsync(OperationAuthorizationRequirement operation, IEntity entity)
        {
            if (!await IsUserAuthorizedAsync(operation, entity))
            {
                throw new UnauthorizedAccessException();
            }
        }

        public Task<bool> IsUserAuthorizedAsync(FormEditContext editContext, IButtonSetup button)
        {
            var handler = _buttonActionHandlerResolver.GetButtonActionHandler(button);

            return IsUserAuthorizedAsync(handler.GetOperation(button, editContext), editContext.Entity);
        }

        public async Task EnsureAuthorizedUserAsync(FormEditContext editContext, IButtonSetup button)
        {
            if (!await IsUserAuthorizedAsync(editContext, button))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
