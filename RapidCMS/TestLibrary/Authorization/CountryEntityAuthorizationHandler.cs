using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.Authorization;
using RapidCMS.Common.Extensions;
using TestLibrary.Entities;

namespace TestLibrary.Authorization
{
    public class CountryEntityAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, CountryEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, CountryEntity resource)
        {
            //if (resource._Id < 4)
            if (requirement.In(Operations.None, Operations.Add, Operations.Pick, Operations.Remove, Operations.Insert))
            {
                context.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }

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
