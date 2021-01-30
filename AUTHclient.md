# Authentication in RapidCMS (client side + API)

RapidCMS does not provide any authentication out-of-the-box, but integrates seamlessly with any
authentication. To get you started, this page provides details about integrating AD authentication in RapidCMS.

## 1. NuGet package

In order for RapidCMS to integrate with remote signins, the [Microsoft.AspNetCore.Components.WebAssembly.Authentication](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.WebAssembly.Authentication)
package is already installed. If you wish to log in with AD, install the [Microsoft.Authentication.WebAssembly.Msal](https://www.nuget.org/packages/Microsoft.Authentication.WebAssembly.Msal) 
package to get started.

## 2a. Active Directory

Add an App Registration to your Active Directory, and add `https://localhost:[your-port]/authentication/login-callback` as
reply url for the Web App. Copy the Application Id GUID, and paste it in the following JSON which needs to
be added to `appsettings.json` in `wwwroot`.

```json
"AzureAd": {
    "Authentication": {
        "Authority": "https://login.microsoftonline.com/[tenant-id]",
        "ClientId": "[client-id]"
    },
    "LoginMode": "redirect",
    // required when interfacing an API
    "DefaultAccessTokenScopes": [
        "api://[api-client-id]/[api-scope]"
    ]
}
```

## 2b. Add code

To the `Main` method in `Program.cs`, add the following code:

```c#
builder.Services.AddAuthorizationCore();

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions);
});
```

To `index.html` in `wwwroot`, add the following script to `<body>`.

```html
<script src="_content/Microsoft.Authentication.WebAssembly.Msal/AuthenticationService.js"></script>
```

## 3a. Use OIDC

In order to support an OpenID Connect identity provider, add the following configuration to `appsettings.json`.

```json
"DevOIDC": {
    "ProviderOptions": {
        "Authority": "[provider-url]",
        "MetadataUrl": "[provider-url]/.well-known/openid-configuration",
        "ClientId": "[client-id]",
        "DefaultScopes": [
            "[api-scope]"
        ],
        "ResponseMode": "fragment",
        "ResponseType": "code"
    },
    "UserOptions": {
        "NameClaim": "name"
    }
},
```

## 3b Add code

To the `Main` method in `Program.cs`, add the following code:

```c#
builder.Services.AddAuthorizationCore();

builder.Services.AddOidcAuthentication(config =>
{
    builder.Configuration.Bind("DevOIDC", config);
});
```

To `index.html` in `wwwroot`, add the following script to `<body>`.

```html
<script src="_content/Microsoft.AspNetCore.Components.WebAssembly.Authentication/AuthenticationService.js"></script>
```

## 4. Add `LoginScreen` component

Add a new Razor component to your project, and call it `LoginScreen`. Paste the following code in that component:

```c#
@inject NavigationManager Navigation

<h4>Hi!</h4>

<p>Please sign in to access the CMS.</p>

<hr />

<a class="btn btn-primary" href="/authentication/login">Login via [identity-provider]</a>
```

## 5. Add 'LoginStatus' component

Add a new Razor component to your project, and call it `LoginStatus`. Paste the following code in that component:

```c#
@using Microsoft.AspNetCore.Components.Authorization

<AuthorizeView>
    <div>
        <div class="btn btn-primary"><i class="icon ion-md-contact"></i> Hi, @context.User.Identity.Name!</div>
        <a class="btn btn-primary" href="/authentication/logout">Logout</a>
    </div>
</AuthorizeView>
```

## 6. Tell ASP.NET and RapidCMS to use those components

Open `Program.cs`,  look up the `builder.Services.AddRapidCMSWebAssembly` method, and add the following instructions:

```c#
// in AddRapidCMSWebAssembly

config.SetCustomLoginScreen(typeof(LoginScreen));
config.SetCustomLoginStatus(typeof(LoginStatus));
```

These instructions tell RapidCMS to use those components as login screen and login status.

Remove `config.AllowAnonymousUser()` to prevent anonymous users to use the CMS.

## 7. (Optional) Configure token forwarding to (RapidCMS) Api companion

If you use an api which should receive the access token for each request to it, you can instruct the HttpClient
which is used by the Api Repository to include the access token. All you have to do is add two instructions:

Open `Program.cs`, and add to `Main`:

```c#
builder.Services.AddRapidCMSApiTokenAuthorization(sp =>
{
    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    handler.ConfigureHandler(new[] { "[base-url-of-api]" });
    return handler;
});
```

This will create a transient `AuthorizationMessageHandler` which will attach the access token from the IdP
to each request to `base-url-of-api`. 

```c#
builder.Services.AddRapidCMSApiRepository<TRepository, AuthorizationMessageHandler>(_baseUri);
```

## 8. (Optional) Configure authorization in the RapidCMS Api companion

Install the [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer) package
in a ASP.NET Core WebApi project. 

Add the following configuration to the `appsettings.json`, this can either be AD or any other OIDC:

```json
"OIDC": {
    "Authority": "[authority url]/[tenant-id]/",
    "TokenValidationParameters": {
      "ValidAudience": "[api-scope]", // for this scope the front-end should request an access token
      "ValidIssuer": "[authority url]/[tenant-id]/" // that trailing slash can sometimes be on the iss-claim (talking to you AAD)
    }
  }
```

Add in `Startup.cs` to `ConfigureServices`:

```c#
services
    .AddAuthorization(o =>
    {
        o.AddPolicy("default", builder =>
    {
        builder.RequireAuthenticatedUser();
    });

services
    .AddAuthentication(o =>
    {
        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        Configuration.Bind("OIDC", options);
    });

services.AddSingleton<IAuthorizationHandler, VeryPermissiveAuthorizationHandler>();
```

The code for `VeryPermissiveAuthorizationHandler` can be found [here](AUTHserver.md);

To `services.AddController` add `config.Filters.Add(new AuthorizeFilter("default"));` to 
only allow authenticated access.

## 9. Hit Run

This is everything you should do to get authentication working in RapidCMS. The two custom components are
used to draw the login screen and the login status in the tob bar. You can fully customize the look and feel of
those components.
