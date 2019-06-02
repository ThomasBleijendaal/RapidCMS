using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Authorization
{
    internal class AllowAllAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, IEntity resource)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
