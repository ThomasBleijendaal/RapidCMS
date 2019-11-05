# Authentication in RapidCMS

RapidCMS does not provide any authentication out-of-the-box, but integrates seamlessly with any
authentication. To get you started, this page provides details about integrating AD authentication in RapidCMS.

## 1. NuGet package

Install the [Microsoft.AspNetCore.Authentication.AzureAD.UI](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.AzureAD.UI/3.0.0) package
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

//services.Configure<CookiePolicyOptions>(options =>
//{
//    // This lambda determines whether user consent for non-essential
//    // cookies is needed for a given request.
//    options.CheckConsentNeeded = context => true;
//    options.MinimumSameSitePolicy = SameSiteMode.None;
//});

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
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapDefaultControllerRoute();
});
```

These methods allow the virtual controllers provided by `AzureAD.UI` work and do their job.

## 4. Add `LoginScreen` component

Add a new Razor component to your project, and call it `LoginScreen`. Paste the following code in that component:

```c#
@using Microsoft.AspNetCore.Http
@inject IHttpContextAccessor _httpContextAccessor

<h4>Hi!</h4>

<a class="btn btn-primary" href="/AzureAD/Account/SignIn?redirectUri=@RedirectUri" target="_top">Login via AD</a>

@code {

    private string RedirectUri;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        RedirectUri = _httpContextAccessor.HttpContext.Request.Host.Value;
    }
}
```

## 5. Add 'LoginStatus' component

Add a new Razor component to your project, and call it `LoginStatus`. Paste the following code in that component:

```c#
@using System.Security.Claims
@using Microsoft.AspNetCore.Http

@inject IHttpContextAccessor _httpContextAccessor

<div>
    @if (User.Identity.Name != null)
    {
        <a class="btn btn-primary" href="#">
            <i class="icon ion-md-person"></i>
            @GivenName
        </a>
        <a class="btn btn-primary" href="/AzureAD/Account/SignOut?post_logout_redirect_uri=@RedirectUri" target="_top">Logout</a>
    }
    else
    {
        <a class="btn btn-primary" href="/AzureAD/Account/SignIn?redirectUri=@RedirectUri" target="_top">Login</a>
    }
</div>

@code {
    private ClaimsPrincipal User;
    private string RedirectUri;
    private string GivenName;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        try
        {
            // Set the user to determine if they are logged in
            User = _httpContextAccessor.HttpContext.User;
            // Try to get the GivenName
            var givenName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.GivenName);
            if (givenName != null)
            {
                GivenName = givenName.Value;
            }
            else
            {
                GivenName = User.Identity.Name ?? "No name";
            }
            // Need to determine where we are to set the RedirectUri
            RedirectUri = _httpContextAccessor.HttpContext.Request.Host.Value;
        }
        catch { }
    }
}
```

## 6. Tell RapidCMS to use those components

Open `Startup.cs`,  look up the `services.AddRapidCMS` method, and add the following instructions:

```c#
config.SetCustomLoginScreen(typeof(LoginScreen));
config.SetCustomLoginStatus(typeof(LoginControl));
```

Remove `config.AllowAnonymousUser()`.

## 7. Hit Run

This is everything you should do to get authentication working in RapidCMS. The two custom components are
used to draw the login screen and the login status in the tob bar. You can fully customize the look and feel of
those components.

The callbacks like `OnTicketReceived` and `OnTokenValidated` can be used to further check which user has signed
in and allow you to onboard any new user. 

## More information

[See this page for more information about Blazor and Authentication](https://devblogs.microsoft.com/aspnet/configuring-a-server-side-blazor-app-with-azure-app-configuration/).