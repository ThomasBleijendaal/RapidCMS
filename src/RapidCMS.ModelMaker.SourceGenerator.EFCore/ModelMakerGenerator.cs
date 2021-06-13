using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders;
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
            var @namespace = "RapidCMS.ModelMaker";

            var propertyParser = new PropertyParser();
            var entityParser = new EntityParser(propertyParser);

            var propertyBuilder = new PropertyBuilder();
            var entityBuilder = new EntityBuilder(@namespace, propertyBuilder); // TODO: config?

            var fieldBuilder = new FieldBuilder();
            var collectionBuilder = new CollectionBuilder(@namespace, fieldBuilder);

            var contextBuilder = new ContextBuilder(@namespace);

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

                        var entitySourceText = entityBuilder.BuildEntity(entity);
                        context.AddSource($"ModelMaker_Entity_{entity.Name}.cs", entitySourceText);

                        var collectionSourceText = collectionBuilder.BuildCollection(entity);
                        context.AddSource($"ModelMaker_Collection_{entity.Name}.cs", collectionSourceText);
                    }
                    // context.ReportDiagnostic(Diagnostic.Create("RC0001", "", json.Value<string>("$type"), DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 1));
                }
            }

            var contextSourceText = contextBuilder.BuildContext(entities);
            context.AddSource("ModelMaker_DbContext", contextSourceText);
        }
    }
}
