# Install Model Maker in RapidCMS client-side

Model Maker is currently not supported in RapidCMS client-side (WebAssembly), but the
generated models and repositories can easily be integrated in a client-side RapidCMS instance.

1. Create a .NET 5.0 class library.
2. Install NuGet-package: `RapidCMS.ModelMaker`.
2. Install NuGet-package: `RapidCMS.ModelMaker.SourceGenerator.EFCore`.
3. Install NuGet-package: `Microsoft.EntityFrameworkCore.SqlServer`.
4. Install NuGet-package: `Microsoft.EntityFrameworkCore.Design`.
5. Install NuGet-package: `Microsoft.EntityFrameworkCore.Tools`.
6. [Create a RapidCMS server-side project](SETUP_SERVERSIDE.md) next to the class library.
7. Install NuGet-package: `RapidCMS.ModelMaker` in the RapidCMS server-side project.
8. Use `config.SetModelFolder` via `.AddModelMaker(configure: config => {})` to save the generated JSON files into the class library's RapidModels folder.
9. Run the Model Maker and create all appropriate models.
10. Make sure all JSON files inside the Model Folder are marked as "C# analyzer additional file".
11. Setup an `IDesignTimeDbContextFactory` in the class library to setup `ModelMakerDbContext` construction during design time (See below).
11. Generate and apply migrations to create database.
12. [Create a RapidCMS client-side project](SETUP_CLIENTSIDE.md) next to the class library.
13. [Create a RapidCMS API companion project](SETUP_COMPANION.md) next to the class library.
14. Add collections, validators and ApiRepositories for all models in the client-side project.
15. Add validators, repositories and DbContext for all models in the api companion project.
16. Add CORS settings so the WebAssembly app can access the API.
17. Run the client-side and companion project to use the generated code for Model Maker models.

The [RapidCMS.Example.ModelMaker.WebAssembly](https://github.com/ThomasBleijendaal/RapidCMS/tree/master/examples/RapidCMS.Example.ModelMaker.WebAssembly) and [RapidCMS.Example.ModelMaker.WebAssembly.API](https://github.com/ThomasBleijendaal/RapidCMS/tree/master/examples/RapidCMS.Example.ModelMaker.WebAssembly.API) demonstrate this setup.


## Design-time DbContext factory

```c#
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ModelMakerDbContext>
{
    public ModelMakerDbContext CreateDbContext(string[] args)
        => new ModelMakerDbContext(
            new DbContextOptionsBuilder<ModelMakerDbContext>()
                .UseSqlServer("{connection string}")
                .Options);
}
```