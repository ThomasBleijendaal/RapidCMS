using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Api.WebApi.Resolvers;

internal class UserResolver : IUserResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? GetUser() 
        => _httpContextAccessor.HttpContext?.User;
}
