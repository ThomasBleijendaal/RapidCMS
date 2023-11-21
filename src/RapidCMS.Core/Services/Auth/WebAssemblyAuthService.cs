using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Services.Auth;

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

    public Task EnsureAuthorizedUserAsync(FormEditContext editContext, ButtonSetup button)
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

    public Task<bool> IsUserAuthorizedAsync(FormEditContext editContext, ButtonSetup button)
    {
        return Task.FromResult(true);
    }
}
