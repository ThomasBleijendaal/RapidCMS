using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Api.Core.Services
{
    internal class ApiAuthService : IAuthService
    {
        private readonly IUserResolver _userResolver;
        private readonly IServiceProvider _serviceProvider;

        public ApiAuthService(
            IUserResolver userResolver,
            IServiceProvider serviceProvider)
        {
            _userResolver = userResolver;
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
            // TODO: why resolve here and not just use DI
            var authorizationService = _serviceProvider.GetRequiredService<IAuthorizationService>();

            var user = _userResolver.GetUser();
            if (user == null)
            {
                return false;
            }

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
