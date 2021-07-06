# Install Model Maker in RapidCMS client-side

Model Maker is currently not supported in RapidCMS client-side (WebAssembly), but the
generated models and repositories can easily be integrated in a client-side RapidCMS instance.

1. Create a .NET 5.0 class library.
2. Install NuGet-package: `RapidCMS.ModelMaker.SourceGenerator.EFCore` (currently in preview).
2. Install NuGet-package: `Microsoft.EntityFrameworkCore.SqlServer`.
2. Install NuGet-package: `Microsoft.EntityFrameworkCore.Design`.
4. [Create a RapidCMS server-side project](SETUP_SERVERSIDE.md) next to the class library.
5. Install NuGet-package: `RapidCMS.ModelMaker` in the RapidCMS server-side project.
6. Use `config.SetModelFolder` via `.AddModelMaker(configure: config => {})` to save the generated JSON files into the class library's RapidModels folder.
7. Run the Model Maker and create all appropriate models.
8. Make sure all JSON files inside the Model Folder are marked as "C# analyzer additional file".
3. Setup an `IDesignTimeDbContextFactory` in the class library to setup `ModelMakerDbContext` construction during design time.
8. [Create a RapidCMS client-side project](SETUP_CLIENTSIDE.md) next to the class library.
9. [Create a RapidCMS API companion project](SETUP_COMPANION.md) next to the class library.
10. Add collections, validators and ApiRepositories for all models in the client-side project.
11. Add validators and repositories for all models in the api companion project.
12. Run the client-side and companion project to use the generated code for Model Maker models.

The RapidCMS.Example.ModelMaker.WebAssembly and RapidCMS.Example.ModelMaker.WebAssembly.API demonstrate
this setup.
