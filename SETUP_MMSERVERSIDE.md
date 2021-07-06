# Install Model Maker in RapidCMS server-side

1. [Create a RapidCMS server-side project](SETUP_SERVERSIDE.md).
2. Install NuGet-package: `RapidCMS.ModelMaker`.
3. Add `services.AddModelMaker()` to `ConfigureServices` before the `AddRapidCMSServer` call.
4. Add `config.AddModelMakerPlugin()` to `AddRapidCMSServer(config => {})`.
5. Hit `F5`: you're now running a RapidCMS instance with Model Maker plugin. 
6. Start building your CMS by configuring collections.

## Install Model Maker Entity Framework Generator

1. Install NuGet-package: `RapidCMS.ModelMaker.SourceGenerator.EFCore`.
2. Install NuGet-package: `Microsoft.EntityFrameworkCore.SqlServer`.
3. Install NuGet-package: `Microsoft.EntityFrameworkCore.Tools`.
4. Make sure all Model Maker JSON files are marked as "C# analyzer additional file".
5. Look under Dependencies > Analyzers > RapidCMS.ModelMaker.SourceGenerator.EFCore to discover all the generated classes.
6. Add generated code to DI and RapidCMS config to finalize CMS.
7. Generate and apply migrations to create database.
8. Hit `F5`: you're now running a RapidCMS instance with generated collections and models.
