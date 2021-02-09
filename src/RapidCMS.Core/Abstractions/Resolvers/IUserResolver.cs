using System.Security.Claims;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IUserResolver
    {
        ClaimsPrincipal? GetUser();
    }
}
