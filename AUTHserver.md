# Authentication in RapidCMS (server side)

RapidCMS does not provide any authentication out-of-the-box, but integrates seamlessly with any
authentication. To get you started, this page provides details about integrating OpenID Connect authentication in RapidCMS.

## 1. NuGet package

Install the [Microsoft.AspNetCore.Authentication.OpenIdConnect](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.OpenIdConnect/5.0.2) package
to get started.

## 2a. Active Directory App Registration

Add an App Registration to your Active Directory, and add `https://localhost:[your-port]/signin-oidc` as
reply url for the Web App. Copy the Application Id GUID, and paste it in the following JSON which needs to
be added to `appsettings.json`.

```
"AzureAd": {
    "Authority": "https://login.microsoftonline.com/[tenant-id]",
    "TenantId": "[tenant-id]",
    "ClientId": "[client-id]"
}
```

## 2b. Or use any other OpenID Connect Identity Provider

```
"DevOIDC": {
    "Authority": "[authority url]",
    "TenantId": "[tenant-id]",
    "MetadataAddress": "http://[authority url]/[tenant-id]/.well-known/openid-configuration",
    "ClientId": "[client-id]",
    "TokenValidationParameters": {
        "NameClaimType": "name", // sets User.Identity.Name to use the "name"-claim
        "ValidIssuer": "[authority url]/[tenant-id]/" // that trailing slash can sometimes be on the iss-claim (talking to you AAD)
    }
}
```

## 3. Add code to `Startup.cs`

To the `ConfigureServices` method in `Startup`, add the following code:

```c#
services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "OpenIdConnect";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("OpenIdConnect", options =>
    {
        Configuration.Bind("[oidc-settings-section-name]", options);

        IdentityModelEventSource.ShowPII = true; // useful during debugging. remove for when auth works

        options.Events.OnSignedOutCallbackRedirect = (ctx) =>
        {
            ctx.HandleResponse();
            ctx.Response.Redirect("/");
            return Task.CompletedTask;
        };
    });
```

Verify that the following instructions are present in `Configure` in `Startup.cs`:

```c#
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});
```

These methods allow the virtual controllers provided by `AzureAD.UI` to do their job, and authentication is enforced.

## 4a. Add `LoginScreen` component

Add a new Razor component to your project, and call it `LoginScreen`. Paste the following code in that component:

```c#
<h4>Hi!</h4>

<p>Please sign in to access the CMS.</p>

<hr />

<a class="btn btn-primary" href="/SignIn" target="_top">Login using [identity provider]</a>
```

## 4b. Add `SignIn.cshtml` Razor Page

The `/SignIn` has to go somewhere, so we need to add a `SignIn.cshtml` to the `Pages` folder:

```cshtml
@page "/SignIn"

@model [namespace].SignInModel
@{
}
```

```cs
public class SignInModel : PageModel
{
    public IActionResult OnGet()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = Url.Content("~/")
        }, "OpenIdConnect");
    }
}
```

## 5a. Add 'LoginStatus' component

Add a new Razor component to your project, and call it `LoginStatus`. Paste the following code in that component:

```c#
<AuthorizeView>
    <div>
        <div class="btn btn-primary"><i class="icon ion-md-contact"></i> Hi, @context.User.Identity.Name!</div>
        <a class="btn btn-primary" href="/SignOut" target="_top">Logout</a>
    </div>
</AuthorizeView>
```

## 5b. Add `SignOut.cshtml` Razor Page

The `/SignOut` also has to go somewhere, wo se need to add a `SignOut.cshtml` to the `Pages` folder:

```cshtml
@page "/SignOut"

@model [namespace].SignOutModel
@{
}
```

```cs
public class SignOutModel : PageModel
{
    public IActionResult OnGet()
    {
        return SignOut(new AuthenticationProperties 
        {
            RedirectUri = Url.Content("~/") 
        }, "Cookies", "OpenIdConnect");
    }
}
```

## 6. Implement a Authorization Handler

Since you are in control of who is allowed to do what, you have to implement an AuthorizationHandler. To get you started, this
is a very permissive handler that allows everyone to do anything:

```c#
public class VeryPermissiveAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IEntity>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, IEntity resource)
    {
        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
```

The `requirement` parameter will always be one of the onces defined in `RapidCMS.Core.Authorization.Operation`, and
are your familiar CRUD operations, like `Create`, `Read`, `Update`, `Delete` and for relations `Add` and `Remove` also
exist. When creating you own handlers you can narrow the scope of each handler by not using `IEntity` but each of your
own entities. Or create a handler for a specific entity, and have the general `IEntity`-handler catch the rest.

## 7. Tell ASP.NET and RapidCMS to use those components

Open `Startup.cs`,  look up the `services.AddRapidCMSServer` method, and add the following instructions:

```c#
// in ConfigureServices
services.AddTransient<IAuthorizationHandler, VeryPermissiveAuthorizationHandler>();

// in AddRapidCMSServer

config.SetCustomLoginScreen(typeof(LoginScreen));
config.SetCustomLoginStatus(typeof(LoginStatus));
```

These instructions tell RapidCMS to use those components as login screen and login status.

Remove `config.AllowAnonymousUser()` to prevent anonymous users to use the CMS.

## 8. Hit Run

This is everything you should do to get authentication working in RapidCMS. The two custom components are
used to draw the login screen and the login status in the tob bar. You can fully customize the look and feel of
those components.

The callbacks like `OnTicketReceived` and `OnTokenValidated` can be used to further check which user has signed
in and allow you to onboard any new user. 
