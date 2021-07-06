# Install Model Maker in RapidCMS server-side

1. [Create a RapidCMS server-side project](SETUP_SERVERSIDE.md).
2a. Install NuGet-package: `RapidCMS.ModelMaker` (currently in preview).
3. Add `services.AddModelMaker()` to `ConfigureServices` before the `AddRapidCMSServer` call.
4. Add `config.AddModelMakerPlugin()` to `AddRapidCMSServer(config => {})`.
5. Hit `F5`: you're now running a RapidCMS instance with Model Maker plugin. 
6. Start building your CMS by configuring collections.

## Install Model Maker Entity Framework Generator

1. Install NuGet-package: `RapidCMS.ModelMaker.SourceGenerator.EFCore` (currently in preview).
2. Restart VS.
3. 
