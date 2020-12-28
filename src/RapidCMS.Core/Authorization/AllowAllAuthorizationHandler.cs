using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Authorization
{
    internal class AllowAllAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, IEntity resource)
        {
            if (context.User.Identity?.AuthenticationType == "anonymous")
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
