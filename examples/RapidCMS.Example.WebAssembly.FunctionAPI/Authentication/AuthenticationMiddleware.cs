using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace RapidCMS.Example.WebAssembly.FunctionAPI.Authentication;

// PREVIEW: this middleware is temporary and should be replaced with something first party when Azure Functions on .NET 5.0 supports better middleware
public class AuthenticationMiddleware
{
    private readonly AuthenticationConfig _authenticationConfig;

    public AuthenticationMiddleware(IOptions<AuthenticationConfig> authenticationConfig)
    {
        _authenticationConfig = authenticationConfig.Value;
    }

    public async Task InvokeAsync(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (GetRequestData(context) is HttpRequestData request)
        {
            try
            {
                var authorizationHeader = request.Headers.FirstOrDefault(x => x.Key.Equals("authorization", StringComparison.InvariantCultureIgnoreCase));
                if (authorizationHeader.Value is IEnumerable<string> headerValue)
                {
                    var user = await GetValidUserAsync(headerValue.FirstOrDefault());
                    if (user != null)
                    {
                        context.Items.Add("User", user);
                        await next(context);
                        return;
                    }
                }
            }
            catch { }

            // NOTE: this middleware requires an authenticated user
            SetRequestResponse(context, request.CreateResponse(HttpStatusCode.Unauthorized));
            return;
        }

        await next(context);
    }

    // PREVIEW: getting stuff via Reflection is bad.
    private static HttpRequestData? GetRequestData(FunctionContext context)
    {
        // Use reflection to grab HttpRequestData
        var keyValuePair = context.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
        var functionBindingsFeature = keyValuePair.Value;
        var type = functionBindingsFeature.GetType();
        var inputData = type.GetProperties().Single(p => p.Name == "InputData").GetValue(functionBindingsFeature) as IReadOnlyDictionary<string, object>;
        return inputData?.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
    }

    // PREVIEW: setting stuff via Reflection is bad.
    private static void SetRequestResponse(FunctionContext context, HttpResponseData response)
    {
        var keyValuePair = context.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
        var functionBindingsFeature = keyValuePair.Value;
        var type = functionBindingsFeature.GetType();
        var result = type.GetProperties().Single(p => p.Name == "InvocationResult");
        result.SetValue(functionBindingsFeature, response);
    }

    public async Task<ClaimsPrincipal> GetValidUserAsync(string? authorizationHeader)
    {
        var configurationManager = BuildConfigurationManager(_authenticationConfig.Authority);
        var accessToken = GetAccessToken(authorizationHeader);

        try
        {
            IdentityModelEventSource.ShowPII = true;

            var oidcWellknownEndpoints = await configurationManager.GetConfigurationAsync();

            var tokenValidator = new JwtSecurityTokenHandler
            {
                MapInboundClaims = _authenticationConfig.Authority.AbsoluteUri.Contains("microsoft")
            };

            var validationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateAudience = true,
                ValidAudience = _authenticationConfig.ValidAudience,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
                ValidIssuer = _authenticationConfig.ValidIssuer
            };

            return tokenValidator.ValidateToken(accessToken, validationParameters, out var securityToken);
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException(ex.Message);
        }
    }

    private static Microsoft.IdentityModel.Protocols.ConfigurationManager<OpenIdConnectConfiguration> BuildConfigurationManager(Uri instanceUri)
    {
        var wellKnownEndpoint = $"{instanceUri}/.well-known/openid-configuration";

        var documentRetriever = new Microsoft.IdentityModel.Protocols.HttpDocumentRetriever()
        {
            RequireHttps = instanceUri.Scheme == "https"
        };

        return new Microsoft.IdentityModel.Protocols.ConfigurationManager<OpenIdConnectConfiguration>(wellKnownEndpoint, new OpenIdConnectConfigurationRetriever(), documentRetriever);
    }

    private static string GetAccessToken(string? authorizationHeader)
    {
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.Contains("Bearer "))
        {
            throw new UnauthorizedAccessException();
        }

        var accessToken = authorizationHeader["Bearer ".Length..];
        return accessToken;
    }
}
