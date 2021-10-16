using System.Security.Claims;
using RapidCMS.Api.Functions.Abstractions;
using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Api.Functions.Resolvers
{
    internal class UserResolver : IUserResolver
    {
        private readonly IFunctionContextAccessor _functionExecutionContextAccessor;

        public UserResolver(
            IFunctionContextAccessor functionExecutionContextAccessor)
        {
            _functionExecutionContextAccessor = functionExecutionContextAccessor;
        }

        public ClaimsPrincipal? GetUser()
            => _functionExecutionContextAccessor.FunctionContext != null &&
                _functionExecutionContextAccessor.FunctionContext.Items.TryGetValue("User", out var userObject) &&
                userObject is ClaimsPrincipal user
                ? user
                : default;
    }
}
