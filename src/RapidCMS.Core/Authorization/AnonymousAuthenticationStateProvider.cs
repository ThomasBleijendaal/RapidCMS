using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace RapidCMS.Core.Authorization
{
    internal class AnonymousAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var id = new ClaimsIdentity("anonymous");
            id.AddClaim(new Claim(ClaimTypes.Name, "Anonymous"));

            var principal = new ClaimsPrincipal(id);

            return Task.FromResult(new AuthenticationState(principal));
        }
    }
}
