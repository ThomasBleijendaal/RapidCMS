# Install RapidCMS client-side

1. Create new ASP.NET Core Blazor WebAssembly project.
2. Install NuGet-package: `RapidCMS.UI`.
3. Add `builder.Services.AddAuthorizationCore();` and `builder.Services.AddRapidCMSWebAssembly(config => { config.AllowAnonymousUser(); })` before the last line of `Main` in `Program.cs`.
4. Replace the contents of `App.razor` with `<RapidCMS.UI.Components.Router.RapidCmsRouter />`.
5. Replace the `<link href="css/site.css" rel="stylesheet" />` tags in `index.html` with `<link href="_content/RapidCMS.UI/css/site.css" rel="stylesheet" />` and remove any other css. Add `<script src="_content/rapidcms.ui/js/interop.js"></script>` at the end of the body tag.
6. Hit `F5`: you're now running a completely empty RapidCMS instance.
7. Start building your CMS by expanding `config => {}`. Read [about all the features](FEATURES.md),
or browse the [Examples](https://github.com/ThomasBleijendaal/RapidCMS/tree/master/examples) to see all the options.
