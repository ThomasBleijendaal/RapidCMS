using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace RapidCMS.Example.WebAssembly.FunctionAPI.Authentication
{
    // this class is temporary
    internal class FunctionContextAuthenticationStateProvider : AuthenticationStateProvider
    {
        public FunctionContextAuthenticationStateProvider()
        {
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var id = new ClaimsIdentity("anonymous");
            id.AddClaim(new Claim(ClaimTypes.Name, "Anonymous"));

            var principal = new ClaimsPrincipal(id);

            return Task.FromResult(new AuthenticationState(principal));
        }
    }
}
