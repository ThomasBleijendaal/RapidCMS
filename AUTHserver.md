# Authentication in RapidCMS (server side)

RapidCMS does not provide any authentication out-of-the-box, but integrates seamlessly with any
authentication. To get you started, this page provides details about integrating AD authentication in RapidCMS.

## 1. NuGet package

Install the [Microsoft.AspNetCore.Authentication.AzureAD.UI](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.AzureAD.UI) package
to get started.

## 2. Active Directory App Registration

Add an App Registration to your Active Directory, and add `https://localhost:[your-port]/signin-oidc` as
reply url for the Web App. Copy the Application Id GUID, and paste it in the following JSON which needs to
be added to `appsettings.json`.

```
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/common",
    "ClientId": "{app registration GUID}",
    "CallbackPath": "/signin-oidc"
},
```

Use `https://login.microsoftonline.com/[tenant-id]` to prevent guest accounts from another Active Directory to
be redirected to the wrong Active Directory instance.

## 3. Add code to `Startup.cs`

To the `ConfigureServices` method in `Startup`, add the following code:

```c#
// ***********************************************
// For more info on:
// Microsoft.AspNetCore.Authentication.AzureAD.UI
// see:
// https://bit.ly/2Fv6Zxp
// This creates a 'virtual' controller 
// called 'Account' in an Area called 'AzureAd' that allows the
// 'AzureAd/Account/SignIn' and 'AzureAd/Account/SignOut'
// links to work
services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
    .AddAzureAD(options => Configuration.Bind("AzureAd", options));

// This configures the 'middleware' pipeline
// This is where code to determine what happens
// when a person logs in is configured and processed
services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Instead of using the default validation 
        // (validating against a single issuer value, as we do in
        // line of business apps), we inject our own multitenant validation logic
        ValidateIssuer = false,
        // If the app is meant to be accessed by entire organizations, 
        // add your issuer validation logic here.
        //IssuerValidator = (issuer, securityToken, validationParameters) => {
        //    if (myIssuerValidationLogic(issuer)) return issuer;
        //}
    };
    options.Events = new OpenIdConnectEvents
    {
        OnTicketReceived = context =>
        {
            // If your authentication logic is based on users 
            // then add your logic here
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            context.Response.Redirect("/Error");
            context.HandleResponse(); // Suppress the exception
            return Task.CompletedTask;
        },
        OnSignedOutCallbackRedirect = context =>
        {
            // This is called when a user logs out
            // redirect them back to the main page
            context.Response.Redirect("/");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        // If your application needs to do authenticate single users, 
        // add your user validation below.
        //OnTokenValidated = context =>
        //{
        //    return myUserValidationLogic(context.Ticket.Principal);
        //}
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
    endpoints.MapDefaultControllerRoute();
});
```

These methods allow the virtual controllers provided by `AzureAD.UI` to do their job, and authentication is enforced.

## 4. Add `LoginScreen` component

Add a new Razor component to your project, and call it `LoginScreen`. Paste the following code in that component:

```c#
<h4>Hi!</h4>

<p>Please sign in to access the CMS.</p>

<hr />

<a class="btn btn-primary" href="/AzureAD/Account/SignIn?redirectUri=/" target="_top">Login via Active Directory</a>
```

## 5. Add 'LoginStatus' component

Add a new Razor component to your project, and call it `LoginStatus`. Paste the following code in that component:

```c#
<AuthorizeView>
    <div>
        <div class="btn btn-primary"><i class="icon ion-md-contact"></i> Hi, @context.User.Identity.Name!</div>
        <a class="btn btn-primary" href="/AzureAD/Account/SignOut?post_logout_redirect_uri=/" target="_top">Logout</a>
    </div>
</AuthorizeView>
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

## More information

[See this page for more information about Blazor and Authentication](https://devblogs.microsoft.com/aspnet/configuring-a-server-side-blazor-app-with-azure-app-configuration/).