# Install Model Maker in RapidCMS server-side

1. [Create a RapidCMS server-side project](SETUP_SERVERSIDE.md).
2. Install NuGet-package: `RapidCMS.ModelMaker.SourceGenerator.EFCore`.
3. Add `services.AddModelMaker()` to `ConfigureServices`.
4. Add `config.AddModelMakerPlugin()` to `AddRapidCMSServer(config => {})`.
5. Hit `F5`: you're now running a RapidCMS instance with Model Maker plugin. 
6. Start building your CMS by configuring collections.