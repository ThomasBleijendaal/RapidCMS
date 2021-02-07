using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace RapidCMS.Example.WebAssembly.FunctionAPI.Authentication
{
    // this middleware is temporary and should be replaced with something first party when Azure Functions on .NET 5.0 becomes GA
    public class AuthenticationMiddleware
    {
        private readonly AuthenticationConfig _authenticationConfig;

        public AuthenticationMiddleware(IOptions<AuthenticationConfig> authenticationConfig)
        {
            _authenticationConfig = authenticationConfig.Value;
        }

        public async Task InvokeAsync(FunctionExecutionContext context, FunctionExecutionDelegate next)
        {
            if (context.InvocationRequest is InvocationRequest invocation)
            {
                var req = invocation.InputData.FirstOrDefault(x => x.Name == "req");

                if (req?.Data?.Http != null)
                {
                    try
                    {
                        var request = new Microsoft.Azure.Functions.Worker.HttpRequestData(req.Data.Http);

                        var authorizationHeader = request.Headers.FirstOrDefault(x => x.Key.Equals("authorization", StringComparison.InvariantCultureIgnoreCase));
                        if (authorizationHeader.Value is string headerValue)
                        {
                            var user = await GetValidUserAsync(headerValue);
                            if (user != null)
                            {
                                context.Items.Add("User", user);
                                await next(context);
                                return;
                            }
                        }
                    }
                    catch { }

                    context.InvocationResult = new HttpResponseData(HttpStatusCode.Unauthorized);
                    return;
                }
            }

            await next(context);
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

        private static ConfigurationManager<OpenIdConnectConfiguration> BuildConfigurationManager(Uri instanceUri)
        {
            var wellKnownEndpoint = $"{instanceUri}/.well-known/openid-configuration";

            var documentRetriever = new HttpDocumentRetriever()
            {
                RequireHttps = instanceUri.Scheme == "https"
            };

            return new ConfigurationManager<OpenIdConnectConfiguration>(wellKnownEndpoint, new OpenIdConnectConfigurationRetriever(), documentRetriever);
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
}
