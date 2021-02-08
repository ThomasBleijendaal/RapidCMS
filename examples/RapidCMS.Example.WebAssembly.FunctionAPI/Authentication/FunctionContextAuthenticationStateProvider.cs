using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using RapidCMS.Api.Functions.Abstractions;

namespace RapidCMS.Example.WebAssembly.FunctionAPI.Authentication
{
    // this class is temporary
    internal class FunctionContextAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IFunctionExecutionContextAccessor _functionExecutionContextAccessor;

        public FunctionContextAuthenticationStateProvider(
            IFunctionExecutionContextAccessor functionExecutionContextAccessor)
        {
            _functionExecutionContextAccessor = functionExecutionContextAccessor;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_functionExecutionContextAccessor.FunctionExecutionContext != null &&
                _functionExecutionContextAccessor.FunctionExecutionContext.Items.TryGetValue("User", out var userObject) && 
                userObject is ClaimsPrincipal user)
            {
                return Task.FromResult(new AuthenticationState(user));
            }

            return Task.FromResult(new AuthenticationState(null));
        }
    }
}
