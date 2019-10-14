using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Providers
{
    internal class AuthenticationStateProvider : IAuthenticationStateProvider
    {
        private readonly CmsConfig _cmsConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationStateProvider(CmsConfig cmsConfig, IHttpContextAccessor httpContextAccessor)
        {
            _cmsConfig = cmsConfig;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<bool> UserIsAuthenticatedAsync()
        {
            if (_cmsConfig.AllowAnonymousUsage)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated);
            }
        }
    }
}
