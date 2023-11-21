using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Api.Core.Services;

internal class ApiAuthService : IAuthService
{
    private readonly IUserResolver _userResolver;
    private readonly IAuthorizationService _authorizationService;

    public ApiAuthService(
        IUserResolver userResolver,
        IAuthorizationService authorizationService)
    {
        _userResolver = userResolver;
        _authorizationService = authorizationService;
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
        var user = _userResolver.GetUser();
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

    public Task<bool> IsUserAuthorizedAsync(FormEditContext editContext, ButtonSetup button)
    {
        throw new NotImplementedException("This method is not implemented in ApiAuthService as it makes no sense here.");
    }

    public async Task EnsureAuthorizedUserAsync(FormEditContext editContext, ButtonSetup button)
    {
        if (!await IsUserAuthorizedAsync(editContext, button))
        {
            throw new UnauthorizedAccessException();
        }
    }
}
