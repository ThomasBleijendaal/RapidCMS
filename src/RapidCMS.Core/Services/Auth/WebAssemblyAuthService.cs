using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Services.Auth
{
    internal class WebAssemblyAuthService : IAuthService
    {
        public Task EnsureAuthorizedUserAsync(UsageType usageType, IEntity entity)
        {
            return Task.CompletedTask;
        }

        public Task EnsureAuthorizedUserAsync(OperationAuthorizationRequirement operation, IEntity entity)
        {
            return Task.CompletedTask;
        }

        public Task EnsureAuthorizedUserAsync(EditContext editContext, IButtonSetup button)
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsUserAuthorizedAsync(UsageType usageType, IEntity entity)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsUserAuthorizedAsync(OperationAuthorizationRequirement operation, IEntity entity)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsUserAuthorizedAsync(EditContext editContext, IButtonSetup button)
        {
            return Task.FromResult(true);
        }
    }
}
