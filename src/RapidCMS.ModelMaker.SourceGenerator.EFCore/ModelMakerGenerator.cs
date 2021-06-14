using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Contexts;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Parsers;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore
{
    [Generator]
    public class ModelMakerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {

        }

        public void Execute(GeneratorExecutionContext context)
        {
            var @namespace = "RapidCMS.ModelMaker"; // TODO: config?

            var propertyParser = new PropertyParser();
            var entityParser = new EntityParser(propertyParser);

            var propertyBuilder = new PropertyBuilder();
            var entityBuilder = new EntityBuilder(propertyBuilder);

            var fieldBuilder = new FieldBuilder();
            var collectionBuilder = new CollectionBuilder(fieldBuilder);

            var contextBuilder = new ContextBuilder();
            var entityTypeConfigurationBuilder = new EntityTypeConfigurationBuilder();

            var repositoryBuilder = new RepositoryBuilder();

            var entities = new List<EntityInformation>();

            foreach (var file in context.AdditionalFiles)
            {
                if (file.GetText() is SourceText text)
                {
                    var jObject = JObject.Parse(text.ToString());

                    var entity = entityParser.ParseEntity(jObject);

                    if (entity.IsValid()) // TODO: unit test
                    {
                        entities.Add(entity);
                    }
                    // context.ReportDiagnostic(Diagnostic.Create("RC0001", "", json.Value<string>("$type"), DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 1));
                }
            }

            var modelMakerContext = new ModelMakerContext(@namespace, entities);

            foreach (var entity in entities)
            {
                entityParser.ProcessEntity(entity, modelMakerContext);
            }

            foreach (var entity in entities)
            {
                var entitySourceText = entityBuilder.BuildEntity(entity, modelMakerContext);
                context.AddSource($"ModelMaker_Entity_{entity.Name?.Replace(" ", "_")}.cs", entitySourceText);

                var collectionSourceText = collectionBuilder.BuildCollection(entity, modelMakerContext);
                context.AddSource($"ModelMaker_Collection_{entity.Name?.Replace(" ", "_")}.cs", collectionSourceText);
            }

            var contextSourceText = contextBuilder.BuildContext(modelMakerContext);
            context.AddSource("ModelMaker_DbContext.cs", contextSourceText);

            foreach (var entity in entities)
            {
                var entityTypeConfigurationSourceText = entityTypeConfigurationBuilder.BuildEntityTypeConfiguration(entity, modelMakerContext);
                context.AddSource($"ModelMaker_EntityTypeConfiguration_{entity.Name?.Replace(" ", "_")}.cs", entityTypeConfigurationSourceText);

                var repositorySourceText = repositoryBuilder.BuildRepository(entity, modelMakerContext);
                context.AddSource($"ModelMaker_Repository_{entity.Name?.Replace(" ", "_")}.cs", repositorySourceText);
            }
        }
    }
}
