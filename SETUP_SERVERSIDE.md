# Install RapidCMS server-side

1. Create a new ASP.NET Core Blazor Server-App project.
2. Install NuGet-package: `RapidCMS.UI`.
3. Add `services.AddRapidCMS(config => { config.AllowAnonymousUser(); })` at the end of `ConfigureServices` in `Startup.cs`.
4. Replace the `<Router>` in `App.razor` with `<RapidCMS.UI.Components.Router.RapidCmsRouter />`.
5. Replace the `<link href="css/site.css" rel="stylesheet" />` tags in `_Host.cshtml` with `<link href="_content/RapidCMS.UI/css/site.css" rel="stylesheet" />` and remove any other css. Add `<script src="_content/rapidcms.ui/js/interop.js"></script>` at the end of the body tag.
6. Hit `F5`: you're now running a completely empty RapidCMS instance. 
7. Start building your CMS by expanding `config => {}`. For reference, browse the [Examples](https://github.com/ThomasBleijendaal/RapidCMS/tree/master/examples) to see all the options.
