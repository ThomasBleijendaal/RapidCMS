using System.Security.Claims;
using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Api.Core.Resolvers;

internal class AnonymousUserResolver : IUserResolver
{
    public ClaimsPrincipal? GetUser()
    {
        var id = new ClaimsIdentity("anonymous");
        id.AddClaim(new Claim(ClaimTypes.Name, "Anonymous"));

        return new ClaimsPrincipal(id);
    }
}
