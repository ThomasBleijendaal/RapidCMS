using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace RapidCMS.Core.Authorization
{
    public class TokenAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public TokenAuthorizationMessageHandler(
            IAccessTokenProvider provider,
            NavigationManager navigation,
            string baseUri)
            : base(provider, navigation)
        {
            ConfigureHandler(new string[] { baseUri });
        }
    }
}
