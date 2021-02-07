using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace RapidCMS.Core.Authorization
{
    // TODO: move to Api.Core
    internal class HttpContextAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;

            if (user == null)
            {
                // TODO: wut
            }

            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
