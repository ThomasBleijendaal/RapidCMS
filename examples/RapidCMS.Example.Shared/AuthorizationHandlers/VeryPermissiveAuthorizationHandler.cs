using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.AuthorizationHandlers
{
    public class VeryPermissiveAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, IEntity resource)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
