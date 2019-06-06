using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.Authorization;
using TestLibrary.Entities;

namespace TestLibrary.Authorization
{
    public class CountryEntityAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, CountryEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, CountryEntity resource)
        {
            context.Succeed(requirement);
            
            return Task.CompletedTask;
        }
    }

    public class PersonEntityAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, PersonEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, PersonEntity resource)
        {
            //if (requirement == Operations.View || requirement == Operations.Create)
            //{
            //    context.Succeed(requirement);
            //}

            return Task.CompletedTask;
        }
    }
}
