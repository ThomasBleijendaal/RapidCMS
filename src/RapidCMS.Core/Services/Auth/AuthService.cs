using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Data;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task EnsureAuthorizedUserAsync(UsageType usageType, IEntity entity)
        {
            return EnsureAuthorizedUserAsync(Operations.GetOperationForUsageType(usageType), entity);
        }

        public async Task EnsureAuthorizedUserAsync(OperationAuthorizationRequirement operation, IEntity entity)
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

        public Task EnsureAuthorizedUserAsync(EditContext editContext, IButtonSetup button)
        {
            return EnsureAuthorizedUserAsync(button.GetOperation(editContext), editContext.Entity);
        }
    }
}
