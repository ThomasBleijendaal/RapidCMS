using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Services.Auth
{
    internal class ApiAuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IServiceProvider _serviceProvider;

        public ApiAuthService(
            AuthenticationStateProvider authenticationStateProvider,
            IServiceProvider serviceProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _serviceProvider = serviceProvider;
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
            var authorizationService = _serviceProvider.GetRequiredService<IAuthorizationService>();

            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            var authorizationChallenge = await authorizationService.AuthorizeAsync(user, entity, operation);

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
            throw new NotImplementedException("This method is not implemented in ApiAuthService as it makes no sense here.");
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
