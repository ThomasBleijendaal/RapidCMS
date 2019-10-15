using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TestLibrary.Entities;

namespace TestLibrary.Authorization
{
    public class PersonEntityAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, PersonEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, PersonEntity resource)
        {
            //if (requirement == Operations.View || requirement == Operations.List || requirement == Operations.Create || requirement == Operations.Insert)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
