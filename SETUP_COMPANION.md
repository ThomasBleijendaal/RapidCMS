# Install RapidCMS Api companion

## Web Api

1. Create a new ASP.NET Core Api project.
2. Install NuGet-package: `RapidCMS.Api.WebApi`.
3. Remove all code from `ConfigureServices` in `Startup.cs` and add the following block of code:

```c#
services.AddRapidCMSWebApi(config => { config.AllowAnonymousUser(); });
services.AddRapidCMSControllers();
```

4. Hit `F5`: you're now running a completely empty RapidCMS companion Api instance.
5. Start building your CMS Api by expanding `config => {}`. [Explore the example API](https://github.com/ThomasBleijendaal/RapidCMS/tree/master/examples/RapidCMS.Example.WebAssembly.API)
to get a sense on how to build such Api and get it working with your RapidCMS WebAssembly instance.

## Function Api (.NET 5.0 isolated function -- still in preview)

1. Create a new .NET 5.0 Azure Functions project (`dotnet-isolated` -- see [this repo](https://github.com/Azure/azure-functions-dotnet-worker)).
2. Install NuGet-package: `RapidCMS.Api.Functions` (preview).
3. Add the following block of code to `ConfigureServices` in `Program.cs`:

```c#
services.AddRapidCMSFunctions(config => { config.AllowAnonymousUser(); });
```

4. Run `func host start` from the Azure Function project root: you're now running a completely empty, serverless RapidCMS companion Api instance.
5. Start building your CMS Api by expanding `config => {}`. [Explore the example Function API](https://github.com/ThomasBleijendaal/RapidCMS/tree/master/examples/RapidCMS.Example.WebAssembly.FunctionAPI)
to get a sense on how to build such Api and get it working with your RapidCMS WebAssembly instance.
