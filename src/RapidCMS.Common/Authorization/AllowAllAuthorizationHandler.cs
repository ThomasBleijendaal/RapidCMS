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
            if (context.User.Identity.AuthenticationType == null)
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
